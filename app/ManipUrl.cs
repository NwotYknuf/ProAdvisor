using System;
using System.Text.RegularExpressions;

public static class ManipUrl {

    /*
     * Ne garde que le nom du site
     * ex : www.pimkie.fr devient pimkie
     *      https://www.pimkie.fr/truc?bidule=lazkeaze devient pimkie
     */
    public static string trimedUrl(string url) {

        Regex extract = new Regex(@"(?:([a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?)\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
        Match match = extract.Match(url);

        if (match.Groups.Count > 1) {
            return match.Groups[1].Value;
        } else {
            return url;
        }
    }

    /*
     * Retire tout ce qui est compris avant www.
     * ex : www.pimkie.fr devient pimkie.fr
     *      https://www.pimkie.fr devient pimkie.fr
     */
    public static string retireWWW(string url) {
        Regex reg = new Regex(@".*www\.(.*)");
        Match match = reg.Match(url);

        if (match.Groups.Count > 1) {
            return match.Groups[1].Value;
        } else {
            return url;
        }
    }

}