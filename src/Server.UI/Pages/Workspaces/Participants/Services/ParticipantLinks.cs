using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.StaticAssets;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public static class ParticipantLinks
{
    public static ParticipantLink Home => new ( "Home", "/pages/workspace/participants");
    public static ParticipantLink All = new(nameof(All), $"{Home.Url}/all");
    public static ParticipantLink Participant(string id) => new(id, $"{Home.Url}/{id}");

    public static ParticipantLink AllPris = new ("All Pri", $"{Home.Url}/pre-release-inventory");

    public static ParticipantLink MovedParticipants = new ("Moved", $"{Home.Url}/moved");

    public static ParticipantLink Transfers = new ("Transfers", $"{Home.Url}/transfers");

    public record ParticipantLink(string Title, string Url);
}

