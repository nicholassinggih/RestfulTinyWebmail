# RESTful Tiny Webmail

A small RESTful application for writing and sending email.



# How to get the code

To get a local copy of the current code, clone it using git:

    $ git clone git://github.com/nicholassinggih/RestfulTinyWebmail.git
    $ cd RestfulTinyWebmail
  
  
# Build the application

If you have not, you need to install:
+ .NET Framework SDK 4.5
+ Visual Studio 2015 with NuGet Package Manager installed

Open the SPAChallenge.sln file in Visual Studio.

Download all missing dependencies:
+ In the Solution Explorer, right click the References item under SPAChallenge project. Choose Manage NuGet Packages.
+ The NuGet pane will show up, and on top there will be a bar with a message "Some NuGet packages are missing from this solution. Click to restore from your online package sources." Click the Restore button, and Visual Studio will automatically download the missing packages.

Right click the SPAChallenge project in the Solution Explorer window. Click Build. 

The next step is to publish the application to a Web Server of your choice.

# Publishing the Web Application to IIS 
1. Create an Application Pool
In IIS Manager, under the Connections panel you will see the name of your current machine. Expand the node, if not expanded. Right click Application Pools, and select Add Application Pool. Enter the Name, and choose .NET CLR Version v4.xx or later. Make sure the Start application pool immediately checkbox is checked. Then press OK.

2. Create a new Web Site
Right click on Sites just beneath Application Pools. Choose Add Website. Enter the Site name to your liking. Choose the Application Pool by clicking the Select button, and choose the one you created earlier. Enter a Physical path, a path to a folder which contain or would contain the files for the application. 

Enter the bindings value according to your needs. Ie. IP Address, protocol, port and host name if you have a DNS Server set up. 

3. Publish the application into the physical path
In Visual Studio, right click the Project name in the Solution Explorer. Choose Publish. Create a new custom Profile for deployment. Choose File System as the Publish method. Paste the Physical path of the Web Site you created in the previous step as the target location. Click Next.

Choose the Release Configuration then click Publish.

# Questions

You may contact me via email: nicholas.singgih@gmail.com if you have any questions.







