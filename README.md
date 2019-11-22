# Quick Start guide

## Prerequisites

* [ngrok](http://ngrok.com/)
* .NET Core 3.0 runtime installed
* Need Azure Account with Azure Speech resource
* Need subscription Key and region from said Azure Speech resource

## Startup

* Open TranscriptionBlogPostDemo.sln
* Open Project Properties for TranscriptionBlogPostDemo
* Under Debug tab take note of the port number (may want to disable ssl for the sake of simplicity)
* in command-line execute `ngrok http --host-header="localhost:<port_number>" http://localhost:<port_number>`
* Link Nexmo Voice application to ngrok tunnel
* Start in IIS express