<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Meerkat.aspx.cs" Inherits="Meerkat.Sitecore.Meerkat" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Meerkat</title>
    <link href="/sitecore/shell/themes/standard/default/WebFramework.css" rel="Stylesheet" />
    <link href="/sitecore/admin/Wizard/UpdateInstallationWizard.css" rel="Stylesheet" />
    <link type="text/css" href="/sitecore/admin/Wizard/PackageGenerationWizard/css/black-tie/jquery-ui-1.8.24.custom.css" rel="stylesheet" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css" />
</head>
<body>
    <form id="form1" runat="server" class="wf-container" autocomplete="on">
        <div class="wf-content">
            <div>
                <h1><b>Meerkat</b> - Use this Tool to Analyse Differences Between Remote Sitecore instances</h1>
                <br />
                <h2>To compare instances:</h2>
                1. First run the Meerkat.Sitecore.Selfhost executable as an Administrator on the instances you need to compare, specifying the URL and port 
                <br />
                2. Fill in the form below specifying the source and target service url and the Full paths to the Sitecore webroots.<br />
                3. Select the Sitecore Database you want to compare<br />
                4. Test the connection and ensure there are no firewall restrictions blocking the connection
                <br />
                5. Specify a date range to analyse<br />
                6. Click analyse. The tool will then find all differences in Sitecore items (Addtions, Changes and Deletes) that the target instance is missing. You can then choose to omit certain items.<br />
                7. Click Generate to produce an update package that can be used to update the target instance.<br />
                <br />

                <h3>Source Meerkat Service URL</h3>
                <label for="sourceProtocol">Protocol: </label>
                <select id="sourceProtocol">
                    <option value="http://">http://</option>
                    <option value="https://">https://</option>
                </select>
                <label for="sourceUrl">URL:</label>
                <input type="text" id="sourceUrl" style="width: 300px" placeholder="e.g. localhost" />

                <label for="sourcePort">Port:  </label>
                <input type="text" id="sourcePort" placeholder="eg. 8081" />

                <h3>Target Meerkat Service URL</h3>
                <label for="targetProtocol">Protocol: </label>
                <select id="targetProtocol">
                    <option value="http://">http://</option>
                    <option value="https://">https://</option>
                </select>
                <label for="targetUrl">URL:</label>

                <input type="text" id="targetUrl"  style="width: 300px" placeholder="e.g. localhost" />

                <label for="targetPort">Port:</label>
                <input type="text" id="targetPort"  placeholder="eg. 8081" />

                <br />
                <br />

                <h3>Webroot of Source Sitecore Instance:</h3>

                <input type="text" id="sourcePath" style="width: 500px" placeholder="e.g. C:\MySitecoreWebroot" />

                <h3>Webroot of Target Sitecore Instance</h3>

                <input type="text" id="targetPath" style="width: 500px" placeholder="e.g. C:\MySitecoreWebroot" />
                <br />
                <br />

                <h3>Sitecore Database</h3>
                <label for="database">Select Database: </label>
                <select id="database">
                    <option value="master">master</option>
                    <option value="core">core</option>
                    <option value="web">web</option>
                </select>

                <input type="button" value="Test Connection" id="testConnection" />

                <br />
                <br />
                <h3>
                    <b>Please choose a Date Range to Analyse Historic Changes on the Source Sitecore instance:</b>
                </h3>

                <label for="startdate">Start date:</label>
                <input type="text" id="startdate" />
                <label for="enddate">End date:   </label>
                <input type="text" id="enddate" />


                <input type="button" id="analyse" value="Analyse Differences" />
                &nbsp;&nbsp;
                 <div id="processingItem"></div>

                <div id="analyseResults" style="display: none">
                    Items To Add
                    <div id="itemsAdd" style="overflow: scroll; height: 150px;">
                    </div>
                    Items To Change
                     <div id="itemsChange" style="overflow: scroll; height: 150px;">
                     </div>
                    Items to Delete
                    <div id="itemsDelete" style="overflow: scroll; height: 150px;">
                    </div>
                    <br />
                    <div id="downloadLink">
                    </div>
                    <br />
                    <input type="button" id="generate" value="Generate Update Package" />
                    <br />
                </div>
            </div>
        </div>
    </form>
    <script type="text/javascript" src="/scripts/jquery-2.1.1.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
    <script src="/scripts/jquery.signalr-2.0.3.js" type="text/javascript"></script>
    <script src="/signalr/hubs" type="text/javascript"></script>
    <script src="/scripts/custom.js" type="text/javascript"></script>
</body>
</html>
