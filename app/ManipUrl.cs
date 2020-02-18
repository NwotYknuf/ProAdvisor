using System;
using System.Text.RegularExpressions;

public static class ManipUrl {

    public static string trimedUrl(string url) {
        //Match uniquement le nom du site pour se débarasser des www et .xyz
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