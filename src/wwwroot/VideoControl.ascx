<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VideoControl.ascx.cs" Inherits="VideoControl" %>

<video width="640" height="480" autoplay="autoplay" autobuffer="autobuffer" controls="controls">
    <source src="<%=Url %>" type='<%=VideoType %>'>
</video>

<br />
<i>Tento přehrávač je pouze experimentální.</i>