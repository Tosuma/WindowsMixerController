# Purpose of Application
The purpose of this application is for a simple terminal interface for the Windows mixer, making it somewhat easier to interact with the volume levels of different applications.

To make the control of volume easier grouping of application has been made possible, where a group has a volume level which it enforces onto all applications in the group.
Note, however, that the group will, at this point, only enforce the volume when you modify the group.

It is also possible to make a `config.json` file at the location of the `.exe` file where it is possible to configure groups, see the section on [config file](#config-file-layout) information about the layot.

The controller will not enforce the volume onto applications that opens *after* the controller was started at this point in time.

# Config File Layout
The configuration json file consists of a group key-value-pair `groups` where the keys are the name of the groups and the value is the information about the group.
A group has 3 fields; `volume: float_value`, `muted: bool`, and `sessions: [string]`.
-  The `volume` is a `float` value in the range `0-100` representing the volume in percent.
-  The `muted` is an **optional** `bool` indicating if the group is to be muted - not the same as volume set to `0`. If this is left out from the group it is evaluated as `false`.
-  The `sessions` is a list of `string` representing the names of applications that is to be placed in this group.
   Note if a specified application is not running it will just ignore it.
   The capitalization of the letters is ignored.

## Example of a config file
```json
"groups": {
    "Music": {
        "volume": 60,
        "sessions": ["spotify"]
    },
    "Voip": {
        "volume": 100,
        "sessions": ["discord"]
    },
    "Please be quiet": {
        "volume": 0,
        "sessions": ["Chrome", "FiReFoX"]
    },
    "Seriously be quiet": {
        "volume": 100,
        "muted`": true,
        "sessions": ["edge", "netflix"]
    }
}
```
