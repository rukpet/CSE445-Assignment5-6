<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerMoneyView.ascx.cs" Inherits="WebApplication1.PlayerMoneyView" %>
<div class="player-money">
    <h4>Player Money</h4>
    <div class="player-money-row">
        <span class="player-money-label">Player ID:</span>
        <asp:Literal runat="server" ID="litPlayerId" />
    </div>
    <div class="player-money-row">
        <span class="player-money-label">Stack:</span>
        <asp:Literal runat="server" ID="litPlayerStack" />
    </div>
</div>
