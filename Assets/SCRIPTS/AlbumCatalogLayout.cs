using System.Collections.Generic;

public static class AlbumCatalogLayout
{
    public const int CoverPage = 1;
    public const int FirstPhotoPage = 2;
    public const int LastPhotoPage = 23;
    public const int FinalPage = 24;
    public const int TotalPhotos = 86;

    public static int GetPhotoCountForPage(int albumPage)
    {
        if (albumPage < FirstPhotoPage || albumPage > LastPhotoPage)
        {
            return 0;
        }

        return albumPage == 7 || albumPage == 12 ? 3 : 4;
    }

    public static IEnumerable<string> GetPhotoIdsForPage(int albumPage)
    {
        int firstPhotoNumber = 1;

        for (int page = FirstPhotoPage; page < albumPage; page++)
        {
            firstPhotoNumber += GetPhotoCountForPage(page);
        }

        int count = GetPhotoCountForPage(albumPage);
        for (int offset = 0; offset < count; offset++)
        {
            yield return FormatPhotoId(firstPhotoNumber + offset);
        }
    }

    public static string FormatPhotoId(int photoNumber)
    {
        return $"photo_{photoNumber:00}";
    }
}
