<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerDeckView.ascx.cs" Inherits="WebApplication1.PlayerDeckView" %>
<div class="player-deck-view">
    <asp:Panel runat="server" ID="pnlContent" Visible="false">
        <h4>Game <asp:Literal runat="server" ID="litGameId" /></h4>
        <p>
            <strong>Stage:</strong> <asp:Literal runat="server" ID="litStage" /> |
            <strong>Pot:</strong> <asp:Literal runat="server" ID="litPot" /> |
            <strong>Current bet:</strong> <asp:Literal runat="server" ID="litCurrentBet" />
        </p>
        <div>
            <strong>Board:</strong>
            <asp:Literal runat="server" ID="litBoard" />
        </div>
        <div style="margin-top:8px;">
            <strong>Players:</strong>
            <asp:Literal runat="server" ID="litPlayers" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlError" Visible="false">
        <asp:Literal runat="server" ID="litError" />
    </asp:Panel>
</div>
