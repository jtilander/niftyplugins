#
# Creates 2010 archive for NiftyPlugins.
#
import zipfile
import os
import sys
import logging
import re
import shutil

QUIET = 1
VS2010 = 0
VS2005 = 0
DEVENV = ''

if 0 and 'VS100COMNTOOLS' in os.environ.keys():
    VS2010 = 1
    DEVENV = os.path.abspath(os.path.join(os.environ['VS100COMNTOOLS'], '..', 'IDE', 'devenv.com'))
elif 'VS80COMNTOOLS' in os.environ.keys():
    VS2005 = 1
    DEVENV = os.path.abspath(os.path.join(os.environ['VS80COMNTOOLS'], '..', 'IDE', 'devenv.com'))
else:
    print "No visual studio found!"

SETUP_PROJECTS = [
   r'NiftySolutionSetup\NiftySolutionSetup.vdproj', 
   r'NiftyPerforceSetup\NiftyPerforceSetup.vdproj', 
]

ASSEMBLY_INFOS = [
    r'NiftySolution\AssemblyInfo.cs',
    r'NiftyPerforce\AssemblyInfo.cs',
]

EXPERIMENTAL_FILES = [
    (r'Build\Experimental_NiftySolution.msi', 'NiftySolution'),
    (r'Build\Experimental_NiftyPerforce.msi', 'NiftyPerforce'),
]

BASE=os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
DISTDIR=os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'Build'))
SOLUTION2010='NiftyPlugins2010.sln'
SOLUTION2005='NiftyPlugins.sln'

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
    if VS2010:
        name, ext = os.path.splitext(solutionfile)
        solutionfile = name + '_VS2010' + ext
        config = config.split('|')[0]
        execute('m "%(solutionfile)s" /t:Clean "/p:Configuration=%(config)s"' % locals())
        return
    if VS2005:
        devenv = DEVENV
        execute( '"%(devenv)s" "%(solutionfile)s" /clean "%(config)s"' % locals() )
        return

def buildSolution(solutionfile, config):
    if VS2010:
        name, ext = os.path.splitext(solutionfile)
        solutionfile = name + '_VS2010' + ext
        config = config.split('|')[0]
        execute('m "%(solutionfile)s" /t:Build "/p:Configuration=%(config)s"' % locals())
        return
    if VS2005:
        devenv = DEVENV
        execute( '"%(devenv)s" "%(solutionfile)s" /build "%(config)s"' % locals(), 0 )
        return

def msbuild(solution, config, command):
    MSBUILD=r'%s\Microsoft.NET\Framework64\v4.0.30319\MsBuild.exe' % os.environ['windir']
    cmdline = '%(MSBUILD)s /nologo /verbosity:minimal "%(solution)s" /t:%(command)s /p:Configuration=%(config)s' % locals()
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
    for vdproj in SETUP_PROJECTS:
        replaceText(vdproj, r'"ProductVersion" = "8:([^"]*)"', versionstring)
        
        projectname = os.path.splitext(os.path.basename(vdproj))[0]
        uuid = generateUUID(projectname + versionstring)
        replaceText(vdproj, r'"ProductCode" = "8:{([^}]+)}"', uuid)
        
    for assembly in ASSEMBLY_INFOS:
        replaceText(assembly, r'AssemblyVersion\("([^"]*)"\)', versionstring)

def moveOutputIntoPlace(versionstring):
    for oldname, newprefix in EXPERIMENTAL_FILES:
        newname = os.path.join(os.path.dirname(oldname), "%s-%s.msi" % (newprefix, versionstring))
        shutil.copyfile(oldname, newname)

def main(args):
    """
Usage: build.py <version>
    """
    
    if len(args) != 1:
        print main.__doc__
        return 1
    
    versionstring = args[0]
    
    if VS2010:
        msbuild(os.path.join(BASE, SOLUTION2010), 'Release', 'Rebuild')
    elif VS2005:
        print "Updating versions to \"%s\"..." % versionstring
        updateVersion(versionstring)
        
        print "Cleaning solution..."
        cleanSolution(os.path.join(BASE, SOLUTION2005), 'Release')
        
        print "Building installers..."
        buildSolution(os.path.join(BASE, SOLUTION2005), 'Release')
        
        print "Publishing the installers..."
        moveOutputIntoPlace(versionstring)

if __name__ == '__main__':
    sys.exit( main(sys.argv[1:]) )
