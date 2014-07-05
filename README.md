Meerkat.Sitecore
================

Allows Sitecore instance's items to be compared historically over HTTP.

This project consists of a WCF Data Service, a client Web application and a console application.
The client Web aplication can be deployed to a Sitecore webroot and is the interface for comparing instances.

The Client Web Application needs to communicate with the WCF Data Service, which could be on the same computer or somewhere 
remote. You can run the service using the Selfhost Console App and specify the port you want to run it on. 
Alternatively you can set the service to run in IIS.


MPORTANT - Post Installation Notes
-----------------------------------
After installation you must add the following non-invasive changes to your Sitecore Web.Config.

Add the following binding redirects in your <runtime> tag - this is needed for signalr to work

 <assemblyIdentity name="Microsoft.Owin" publicKeyToken="31bf3856ad364e35" culture="neutral">
      <dependentAssembly>
        <bindingRedirect oldVersion="0.0.0.0-2.0.2.0" newVersion="2.0.2.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin.Security" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.0.2.0" newVersion="2.0.2.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.AspNet.SignalR.Core" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.0.3.0" newVersion="2.0.3.0" />
      </dependentAssembly>
    </assemblyBinding>

Then the entry /signalr to the end of the  IgnoreUrlPrefixes setting


<setting name="IgnoreUrlPrefixes" value="......................|/signalr" />

VERY IMPORTANT
-----------------------------
To compare items you need to download and run the Console Application - Meerkat.Sitecore.SelfHost.exe 

Using Administrator Privelages on the server where your Sitecore instances 
live!!

Ensure the port you choose is not blocked by Windows Firewall and you need to manually create a rule to allow the Service to be run.



