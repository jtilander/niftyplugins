#
# Creates 2010 archive for NiftyPlugins.
#
import zipfile
import os
import sys

QUIET = 0
VS2010 = 0
VS2005 = 0
DEVENV = ''

if 'VS100COMNTOOLS' in os.environ.keys():
    VS2010 = 1
    DEVENV = os.path.abspath(os.path.join(os.environ['VS100COMNTOOLS'], '..', 'IDE', 'devenv.com'))
elif 'VS80COMNTOOLS' in os.environ.keys():
    VS2005 = 1
    DEVENV = os.path.abspath(os.path.join(os.environ['VS80COMNTOOLS'], '..', 'IDE', 'devenv.com'))
else:
    print "No visual studio found!"

# (source name, dest dir)
FILES=[
    (r'README', ''),
    (r'COPYING', ''),
    (r'AUTHORS', ''),
    (r'NiftySolution\bin\NiftySolution.dll', ''),
    (r'NiftyPerforce\bin\NiftyPerforce.dll', ''),
    (r'NiftyPerforce\bin\AuroraCore.dll', ''),
    #(r'NiftySolution\NiftySolution2010.AddIn', ''),
    #(r'NiftyPerforce\NiftyPerforce2010.AddIn', ''),
    (r'NiftySolution\NiftySolution.AddIn', ''),
    (r'NiftyPerforce\NiftyPerforce.AddIn', ''),
    (r'NiftySolution\bin\en-US\NiftySolution.resources.dll', 'en-US'),
    (r'NiftyPerforce\bin\en-US\NiftyPerforce.resources.dll', 'en-US'),
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

def makearchive(filename):
    z = zipfile.ZipFile(filename, 'w')
    for name, destdir in FILES:
        fullname = os.path.join(BASE, name)
        basename = os.path.basename(fullname)
        destname = os.path.join(destdir, basename)
        
        z.write(fullname, destname)
    z.close()

def main(args):
    """
Usage: build.py <version>
    """
    
    if len(args) != 1:
        print main.__doc__
        return 1
    
    version = args[0]
    
    if VS2010:
        msbuild(os.path.join(BASE, SOLUTION2010), 'Release', 'Rebuild')
    elif VS2005:
        cleanSolution(os.path.join(BASE, SOLUTION2005), 'Release')
        buildSolution(os.path.join(BASE, SOLUTION2005), 'Release')
    
    makearchive(os.path.join(DISTDIR, 'NiftyPlugins-%s.zip' % version))

if __name__ == '__main__':
    sys.exit( main(sys.argv[1:]) )
