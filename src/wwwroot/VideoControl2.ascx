<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VideoControl2.ascx.cs" Inherits="VideoControl" %>

<object classid="clsid:67DABFBF-D0AB-41fa-9C46-CC0F21721616" width="640" height="500" codebase="http://go.divx.com/plugin/DivXBrowserPlugin.cab">

<param name="custommode" value="none" />

<param name="src" value="<%=Url%>" />

<embed type="video/divx" src="<%=Url%>" custommode="none" width="640" height="500" pluginspage="http://go.divx.com/plugin/download/">
</embed>
</object>
<br>Nezobrazuje se video? Stáhněte si potřebný plugin pro <a style="text-decoration: underline;" href="http://download.divx.com/player/DivXWebPlayerInstaller.exe">Windows</a> nebo <a style="text-decoration: underline;" href="http://download.divx.com/player/DivXWebPlayer.dmg">Mac</a>
