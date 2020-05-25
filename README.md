# Unity Cloud Deployer
Tool to automatically deploy your WebGL games from UnityCloud to your ASP.NET server.

## How it works
It is intended to be used along with Unity Cloud Builder, wich can be set to fire an action on builds success. The webhook is something like this:
```json
{
  "buildNumber": {BUILD_NUMBER},
  "buildStatus": "success",
  "buildTargetName": "Default WebGL",
  "cleanBuild": false,
  "credentialsOutdated": false,
  "lastBuiltRevision": {LAST_COMMIT},
  "links": {
    "api_self": {
      "href": "...",
      "method": "get"
    },
    "api_share": {
      "href": "...",
      "method": "get"
    },
    "artifacts": [
      {
        "files": [
          {
            "filename": "Default WebGL.zip",
            "href": {DOWNLOAD_LINK}
          }
        ],
        "key": "primary",
        "name": ".ZIP file",
        "primary": true,
        "show_download": true
      }
    ],
    "dashboard_download": {
      "href": "...",
      "method": "get"
    },
    "dashboard_download_direct": {
      "href": "...",
      "method": "get"
    },
    "dashboard_log": {
      "href": "...",
      "method": "get"
    },
    "dashboard_project": {
      "href": "...",
      "method": "get"
    },
    "dashboard_summary": {
      "href": "...",
      "method": "get"
    },
    "dashboard_url": {
      "href": "https://developer.cloud.unity3d.com",
      "method": "get"
    },
    "share_url": {
      "href": "...",
      "method": "get"
    }
  },
  "local": false,
  "orgForeignKey": {ORG_KEY},
  "platform": "webgl",
  "platformName": "Web GL",
  "projectGuid": {PROJ_ID},
  "projectName": {PROJ_NAME},
  "scmType": "oauth",
  "startedBy": {NAME}
}
```

The script parses the JSON for the download link, downloads and unzips the file and cleans up afterwards.
There is also a logging functionality for debug purposes.

## How to install
### IIS
(IIS instructions)
