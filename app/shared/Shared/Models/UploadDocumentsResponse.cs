﻿namespace Shared.Models;

public record UploadDocumentsResponse(
    string[] UploadedFiles,
    string? Error = null)
{
    public bool IsSuccessful => this is
    {
        Error: null,
        UploadedFiles.Length: > 0
    };

    public static UploadDocumentsResponse FromError(string error) =>
        new([], error);
}