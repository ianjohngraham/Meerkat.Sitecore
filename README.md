Meerkat.Sitecore
================

Allows Sitecore instance's items to be compared historically over HTTP.

This project consists of a WCF data service (Meerkat.Sitecore.Service), a client Web application (Meerkat.Sitecore.Client) and a console application (Meerkat.Sitecore.SelfHost).
Meerkat.Sitecore.Client can be deployed to a Sitecore webroot and is the interface for comparing instances.

The client web applciation needs to communicate with the WCF Data Service, which could be on the same computer or somewhere 
remote. You can run the service using the Selfhost Console App and specify the port you want to run it on. 
Alternatively you can set the service to run in IIS.


MPORTANT - Post Installation Notes
-----------------------------------
After installation you must add the following non-invasive changes to your Sitecore Web.Config.

Add the following binding redirects in your <runtime> tag - this is needed for signalr to work


 &lt;assemblyIdentity name=&quot;Microsoft.Owin&quot; publicKeyToken=&quot;31bf3856ad364e35&quot; culture=&quot;neutral&quot;&gt;
      &lt;dependentAssembly&gt;
        &lt;bindingRedirect oldVersion=&quot;0.0.0.0-2.0.2.0&quot; newVersion=&quot;2.0.2.0&quot; /&gt;
      &lt;/dependentAssembly&gt;
      &lt;dependentAssembly&gt;
        &lt;assemblyIdentity name=&quot;Microsoft.Owin.Security&quot; publicKeyToken=&quot;31bf3856ad364e35&quot; culture=&quot;neutral&quot; /&gt;
        &lt;bindingRedirect oldVersion=&quot;0.0.0.0-2.0.2.0&quot; newVersion=&quot;2.0.2.0&quot; /&gt;
      &lt;/dependentAssembly&gt;
      &lt;dependentAssembly&gt;
        &lt;assemblyIdentity name=&quot;Microsoft.AspNet.SignalR.Core&quot; publicKeyToken=&quot;31bf3856ad364e35&quot; culture=&quot;neutral&quot; /&gt;
        &lt;bindingRedirect oldVersion=&quot;0.0.0.0-2.0.3.0&quot; newVersion=&quot;2.0.3.0&quot; /&gt;
      &lt;/dependentAssembly&gt;
    &lt;/assemblyBinding&gt;



Then the entry /signalr to the end of the  IgnoreUrlPrefixes setting

&lt;setting name=&quot;IgnoreUrlPrefixes&quot; value=&quot;......................|/signalr&quot; /&gt;

VERY IMPORTANT
-----------------------------
To compare items you need to download and run the Console Application (Meerkat.Sitecore.SelfHost.exe). 
using Administrator Privelages on the server where your Sitecore instances 
live!!

Ensure the port you choose is not blocked by Windows Firewall and you need to manually create a rule to allow the service to be run.



