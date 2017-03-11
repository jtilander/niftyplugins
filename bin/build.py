#
# Creates 2010 archive for NiftyPlugins.
#
import zipfile
import os
import sys
import logging
import re
import shutil

QUIET = 0
DEVENV = ''

if 'VS140COMNTOOLS' in os.environ.keys():
	DEVENV = os.path.abspath(os.path.join(os.environ['VS140COMNTOOLS'], '..', 'IDE', 'devenv.com'))
else:
	print "No visual studio found!"
	sys.exit(1)

SETUP_PROJECTS = [
	r'NiftyPerforce\source.extension.vsixmanifest', 
	r'NiftySolution\source.extension.vsixmanifest', 
]

ASSEMBLY_INFOS = [
	r'NiftySolution\AssemblyInfo.cs',
	r'NiftyPerforce\AssemblyInfo.cs',
]

EXPERIMENTAL_FILES = [
	(r'NiftyPerforce\bin\Release\NiftyPerforce.vsix', 'NiftyPerforce'),
	(r'NiftySolution\bin\Release\NiftySolution.vsix', 'NiftySolution'),
]

BASE=os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
DISTDIR=os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'Build'))
SOLUTION='NiftyPlugins.sln'

TARGET_DIR = os.path.abspath( os.path.join(os.path.dirname( __file__ ), '..', 'Build') )

def execute(commandline, silent = 1):
	"""
		Executes a command line and if the command fails, exits.
	"""
	if QUIET:
		silent = 1
		
	logfile = os.path.abspath(os.path.join(os.environ['TEMP'], 'capture_tmp.txt'))
	batchfile = os.path.abspath(os.path.join(os.environ['TEMP'], 'capture_tmp.cmd'))

	try:
		os.unlink(logfile)
	except WindowsError:
		pass
		
	if silent:
		open(batchfile, 'wt').write(
			'@echo off\n\n%s > "%s" 2>&1' % (commandline, logfile)
		)
	else:
		open(batchfile, 'wt').write(
			'%s' % (commandline)
		)
		
	result = os.system(batchfile)
	# We failed. Damn.
	if 0 != result:
		lines = open(logfile).readlines()
		filtered = []
		pattern = re.compile('error ([^:]+):', re.I)
		for line in lines:
			if pattern.search(line):
				filtered.append(line)
		
		if silent:
			print '>>> The output from the program was: '
			print ''.join(filtered)
			print '>>> execution stopped due to error (truncated output above)'
		else:
			print '>>> execution stopped due to error'
		sys.exit(result)
		
def cleanSolution(solutionfile, config):
	devenv = DEVENV
	execute( '"%(devenv)s" "%(solutionfile)s" /clean "%(config)s"' % locals() )

	
def buildSolution(solutionfile, config):
	devenv = DEVENV
	execute( '"%(devenv)s" "%(solutionfile)s" /build "%(config)s"' % locals(), 0 )

	
def msbuild(solution, config, command):
	MSBUILD=r'%s\Microsoft.NET\Framework64\v4.0.30319\MsBuild.exe' % os.environ['windir']
	cmdline = '%(MSBUILD)s /nologo /verbosity:minimal "%(solution)s" /t:%(command)s /p:Configuration="%(config)s"' % locals()
	res = os.system(cmdline)
	if 0 != res:
		print 'Failed to execute %s' % cmdline
		print 'Result: %s' % res
		sys.exit(res)

def generateUUID(name):
	text = name
	import md5
	hexdigest = md5.new( text ).hexdigest()
	# HexDigest is 32 hex characters long, the UUID is 8-4-4-4-12 characters long == 32.
	result = '%s-%s-%s-%s-%s' % (hexdigest[0:8], hexdigest[8:12], hexdigest[12:16], hexdigest[16:20], hexdigest[20:32])
	return result.upper()

def replaceText(filename, pattern, versionstring):
	#print "Opening %s" % filename
	data = open(filename, 'rt').read()
	
	p = re.compile(pattern, re.I)
	
	result = ''
	prev = 0
	for m in p.finditer(data):
		#print "Found hit: %s" % m.group(0)
		start = m.start(1)
		end = m.end(1)
		groupend = m.end(0)
		result = result + data[prev:start] + versionstring + data[end:groupend]
		
		prev = groupend
	
	result = result + data[prev:]
	open(filename,'wt').write(result)

def updateVersion(versionstring):
	for csproj in SETUP_PROJECTS:
		replaceText(csproj, r'<Identity Id=.*Version="([^"]*)"', versionstring)
		
		#projectname = os.path.splitext(os.path.basename(vdproj))[0]
		#uuid = generateUUID(projectname + versionstring)
		#replaceText(vdproj, r'"ProductCode" = "8:{([^}]+)}"', uuid)
		
	for assembly in ASSEMBLY_INFOS:
		replaceText(assembly, r'AssemblyVersion\("([^"]*)"\)', versionstring)

def moveOutputIntoPlace(versionstring):
	if not os.path.isdir(TARGET_DIR):
		os.makedirs(TARGET_DIR)
	for oldname, newprefix in EXPERIMENTAL_FILES:
		newname = os.path.join(TARGET_DIR, "%s-%s.vsix" % (newprefix, versionstring))
		shutil.copyfile(oldname, newname)
		print "%s -> %s" % (oldname, newname)

def main(args):
	"""
Usage: build.py <version>
	"""
	
	if len(args) != 1:
		print main.__doc__
		return 1
	
	versionstring = args[0]
	
	print "Updating versions to \"%s\"..." % versionstring
	updateVersion(versionstring)
	
	print "Cleaning solution..."
	cleanSolution(os.path.join(BASE, SOLUTION), 'Release')
	
	print "Building installers..."
	buildSolution(os.path.join(BASE, SOLUTION), 'Release')
	
	print "Publishing the installers..."
	moveOutputIntoPlace(versionstring)

if __name__ == '__main__':
	sys.exit( main(sys.argv[1:]) )
