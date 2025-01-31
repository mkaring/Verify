﻿using System.Security.Claims;

class ClaimConverter :
    WriteOnlyJsonConverter<Claim>
{
    public override void Write(VerifyJsonWriter writer, Claim claim)
    {
        writer.WriteStartObject();

        var type = claim.Type
            .Replace("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/", "")
            .Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/", "")
            .Replace("http://schemas.microsoft.com/ws/2008/06/identity/claims/", "");
        writer.WriteMember(claim, claim.Value, type);

        writer.WriteMember(claim, claim.Properties, "Properties");

        if (claim.Subject is {Name: { }})
        {
            writer.WriteMember(claim, claim.Subject, "Subject");
        }

        writer.WriteEndObject();
    }
}