<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerMoneyView.ascx.cs" Inherits="WebApplication1.PlayerMoneyView" %>
<div class="player-money">
    <pre>
------------------------------------------
|   O       Player ID: <asp:Literal runat="server" ID="litPlayerId" />
|  /|\      Stack: <asp:Literal runat="server" ID="litPlayerStack" /> chips
|  / \      Deck: [&spades;][&hearts;][&diams;][&clubs;] ready to wager
------------------------------------------
    </pre>
</div>
