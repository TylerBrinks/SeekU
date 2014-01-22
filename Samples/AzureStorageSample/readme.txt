This sample shows how snapshots can be stored in Azure blobs.  In order to run the sample you'll need
to crate an Azure storage account.  Enter your storage account connection string in the the Program.cs
file's SeekU host configuration section.  Running the sample will create the appropriate blob container
for you and upload a snapshot based on the aggregate root's ID.