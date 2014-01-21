This sample shows an MVC website sending commands via a Windows Azure Service Bus.  In order 
to run this sample you'll need to create a Windows Azure Service Bus.  Once you've created
your service bus endpoing you'll need to update the web.config file and the 
ServiceConfiguration.Local.cscfg files with your ACS Connection String.  

After you've updated both files to point to your service bus, you need to set both the 
SampleWebsite and the SampleAzureMessageProcessor projects to start up (Right click the 
solution and select multiple startup projects and set both to start).

When the website and Azure Emulator load, click the "Create a new account" button.  Doing 
so will send a message through the service bus.  The emulator will respond to the message
and marshal the message through the default in-memory SeekU providers.  Check the 
emulator's console output for confirmation that commands are indeed being issued.

You'll also notice that creating a new redirects the website to an async action that 
waits for 2 seconds before showing the web page.  This is a sample lifed from the 
CQRS Journey project.  It's a nice snippet of code that shows how you can poll the 
read model repository to wait for the database to be updated after you issue a command.

There's no actual read model in this example; it's just being simulated.  The take away
is the command being send via Azure to be handled by a long running process.