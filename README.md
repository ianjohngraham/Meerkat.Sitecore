Meerkat.Sitecore
================

Allows Sitecore instance's items to be compared historically over HTTP.

This project consists of a WCF data service (Meerkat.Sitecore.Service), a client web application (Meerkat.Sitecore.Client) and a console application (Meerkat.Sitecore.SelfHost).

You can find all the information you need here

http://coreblimey.azurewebsites.net/?p=1971


IMPORTANT - Pre Installation Notes
-----------------------------------
Before installation you must add the following non-invasive changes to your Sitecore Web.Config.

Add the following binding redirects in your <runtime> tag - this is needed for signalr to work


    &lt;assemblyBinding&gt;
      &lt;dependentAssembly&gt;
        &lt;assemblyIdentity name=&quot;Microsoft.Owin&quot; publicKeyToken=&quot;31bf3856ad364e35&quot; culture=&quot;neutral&quot; /&gt;
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



