<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerMoneyView.ascx.cs" Inherits="WebApplication1.PlayerMoneyView" %>
<div class="player-money">
    <pre>
┌───────────── Player Bank ─────────────┐
│   O       Player ID: <asp:Literal runat="server" ID="litPlayerId" />
│  /|\\      Stack: <asp:Literal runat="server" ID="litPlayerStack" /> chips
│  / \\     Deck: [♠][♥][♦][♣] ready to wager
└────────────────────────────────────────┘
    </pre>
</div>
