
var compareHub;
var comparer;
var jq;

//Add init class pass in Jquery and Document to avoid any conflicts
(function initCustom(j,doc) {
    jq = j;

    jq(function () {
        comparer = {};
        comparer.compareHub = jq.connection.itemComparerHub;

        jq(function () {
            jq("#startdate").datepicker({ dateFormat: "dd/mm/yy" });

        });

        jq(function () {
            jq("#enddate").datepicker({ dateFormat: "dd/mm/yy" });
        });

        jq("#analyse").click(function () {
            analyse();
        });

        jq("#generate").click(function () {
            generate();
        });

        jq("#testConnection").click(function () {
            testConnection();
        });

        jq(doc).on('click','.deselect',function () {
            var ischecked = this.checked;
            $(this).parent().parent().find("input").each(function () {
                if (ischecked) {
                    $(this).prop('checked', true);
                }
                else {
                    $(this).prop('checked', false);
                }
            });
        });

        jq("#generate").click(function () {
            generate();
        });

        comparer.compareHub.client.broadcastMessage = function (message) {
            jq("#processingItem").html("<p>" + message + "</p>");
        };

        comparer.compareHub.client.addCommand = function (path) {
            jq("#itemsAdd").append("<p><input type=\"checkbox\" path='" + path + "' checked=\"true\" />" + path + "</p>");
        };

        comparer.compareHub.client.updateCommand = function (path) {
            jq("#itemsChange").append("<p><input type=\"checkbox\" path='" + path + "' checked=\"true\" />" + path + "</p>");
        };

        comparer.compareHub.client.deleteCommand = function (path) {

            jq("#itemsDelete").append("<p><input type=\"checkbox\" path='" + path + "' checked=\"true\" />" + path + "</p>");
        };

        comparer.compareHub.client.showDownload = function (message) {
            jq("#downloadLink").html("<a href='/Meerkat.aspx?zipfile=" + encodeURI(message) + "'>Download Update Package</a>");
        };
    });

}(jQuery,document));


function stopHub() {
    jq.connection.hub.stop();
    jq("#processingItem").html('');
    jq("#processingItem").html("<p>Compare Stopped!</p>");
}


function testConnection() {
    comparer.sourceUrl = getUrl("source");
    comparer.targetUrl = getUrl("target");
    comparer.sourcePath = getPath("source");
    comparer.targetPath = getPath("target");
    comparer.database = getDatabase();

    jq.connection.hub.start({ transport: 'longPolling' }).done(function () {
        jq("#processingItem").html('');
        comparer.compareHub.server.testConnection(comparer.sourceUrl, comparer.sourcePath, comparer.targetUrl, comparer.targetPath, comparer.database);
    });
}


function analyse() {

    var startdate = jq("#startdate").val();
    startdate = startdate;
    comparer.startdate = startdate;
    var enddate = jq("#enddate").val();
    enddate = enddate;
    comparer.enddate = enddate;
    comparer.sourceUrl = getUrl("source");
    comparer.targetUrl = getUrl("target");
    comparer.sourcePath = getPath("source");
    comparer.targetPath = getPath("target");
    comparer.database = getDatabase();
    jq.connection.hub.start().done(function () {
        jq("#processingItem").html('');
        jq("#itemsAdd").html('');
        jq("#itemsChange").html('');
        jq("#itemsDelete").html('');
        $("#analyseResults").show();
        jq("#itemsAdd").append("<p><input type=\"checkbox\" id='addSelectAll' checked=\"true\" class='deselect' />All</p>")
        jq("#itemsChange").append("<p><input type=\"checkbox\" id='changeSelectAll' checked=\"true\" class='deselect' />All</p>")
        jq("#itemsDelete").append("<p><input type=\"checkbox\" id='deleteSelectAll' checked=\"true\" class='deselect' />All</p>")
        comparer.compareHub.server.analyse(comparer.targetUrl, comparer.sourceUrl, comparer.sourcePath, comparer.targetPath, comparer.startdate, comparer.enddate, comparer.database);
    });
}

function generate() {

    var paths = [];

    jq("#itemsAdd").find("input:checked").each(function () {
        paths.push($(this).attr('path'));
    });
    jq("#itemsChange").find("input:checked").each(function () {
        paths.push($(this).attr('path'));
    });
    jq("#itemsDelete").find("input:checked").each(function () {
        paths.push($(this).attr('path'));
    });

    jq.connection.hub.start({ transport: 'longPolling' }).done(function () {
        jq("#processingItem").html('');
        comparer.compareHub.server.generate(paths, getPath("source"), getPath("target"));
    });
}

function getUrl(instance) {
    var selector = "#" + instance;
    var url = jq(selector + "Url").val();
    var protocol = jq(selector + "Protocol").val();
    var port = jq(selector + "Port").val();
    url = encodeURIComponent(url);
    return protocol + url + ":" + port + "/ItemComparerService.svc";
}


function getPath(instance) {
    var selector = "#" + instance;
    var path = jq(selector + "Path").val();
    path = path.replace("\\", "\\");
    return path;
}


function getDatabase() {
    var database = jq("#database").find('option:selected').attr('value');
    return database;
}




