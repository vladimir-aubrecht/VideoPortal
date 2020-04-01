<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeFile="Serialy.aspx.cs" Inherits="Serialy" %>
<%@ Register TagPrefix="Video" TagName="Player" Src="VideoControl.ascx" %>
<%@ Register TagPrefix="Video" TagName="Player2" Src="VideoControl2.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <center>
        <Video:Player2 runat="server" id="player1" Visible="false" />
        <Video:Player runat="server" id="player" Visible="false" />
    </center>

    <br />

    <a href="FAQ.html">FAQ</a> | 
    <asp:LinkButton ID="lbtnParentFolder" runat="server">O úroveň výše</asp:LinkButton> |
    <asp:LinkButton ID="LinkButton1" runat="server" PostBackUrl="~/Serialy.aspx">Zpět na výběr serialu</asp:LinkButton>
    <br />
    <br />
    
    <asp:MultiView runat="server" ID="Views" ActiveViewIndex="0">
        <asp:View runat="server" ID="TVShowsView">
            
        </asp:View>
    </asp:MultiView>
    
</asp:Content>

