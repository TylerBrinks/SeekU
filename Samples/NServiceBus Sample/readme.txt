This sample shows one method of using NServiceBus as a transport for commands.  You could
just as easily use NServiceBus for trasnporting events instead of or in addition to 
commands.  The implementation is totally up to you.  

The sample works by using a custom ICommandBus implementation in the NSBClientSample that 
sends commands to the NSBServerSample.  The NSBServerSample uses the standard in-memory
providers to issue commands to the appropriate aggregate root instances.

In order to run the sample you need to set both NSBClientSample and NSBServerSample as startup
projects (Right click the solution and select multiple startup projects and set both to start).

You'll also need to have MSMQ installed in order for the sample to run. 
http://msdn.microsoft.com/en-us/library/aa967729(v=vs.110).aspx

Finally, I find that NServiceBus is a bit touchy.  For some reason I sometimes have to run
the solution multiple times for both client and server to start up fully.  This sample
is using the most basic example code directly off of their website....

http://msdn.microsoft.com/en-us/library/aa967729(v=vs.110).aspx