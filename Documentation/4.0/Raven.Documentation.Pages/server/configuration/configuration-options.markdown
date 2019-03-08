# Configuration Options

RavenDB is **Safe by Default** which means its set of options are configured for the best safety.  
However, these options can be manually configured in order to accommodate different server behavior.

{PANEL:Environment Variables}

Configuration can be adjusted using environment variables. Server is going to pick up all environment variables preceded by `RAVEN_` prefix and apply their values to specified configuration keys. All period `.` characters in configuration keys should be replaced with an underscore character (`_`) when used in environment variables. 

### Example

{CODE-BLOCK:plain}
RAVEN_Setup_Mode=None
RAVEN_DataDir=RavenData
RAVEN_Certificate_Path=/config/raven-server.certificate.pfx
{CODE-BLOCK/}

{PANEL/}

{PANEL:JSON}

The `settings.json` file which can be found in the same directory as the server executable can also be used to change the configuration of the server. 
The file is read and applied on the server startup only. It is created when running the server for the first time from the `settings.default.json` file.

### Example

{CODE-BLOCK:json}
{
    "ServerUrl": "http://127.0.0.1:8080",
    "Setup.Mode": "None"
}
{CODE-BLOCK/}

{NOTE Changes in `settings.json` override the environment variables settings. /}

{PANEL/}

{PANEL:Command Line Arguments}

The server can be configured using command line arguments that can be passed to the console application (or while running as a deamon).

### Example:

{CODE-BLOCK:bash}
./Raven.Server --Setup.Mode=None
{CODE-BLOCK/}

{NOTE These command line arguments override the settings of environment variables and the `settings.json`. More details about Command Line Arguments can be found [here](../../server/configuration/command-line-arguments). /}

{PANEL/}

## Related articles

### Configuration

- [Command Line Arguments](../../server/configuration/command-line-arguments)

### Administration

- [Command Line Interface (CLI)](../../server/administration/cli)
