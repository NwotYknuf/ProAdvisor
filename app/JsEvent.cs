using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ProAdvisor.app {

    public static class JsEvent {

        /*
         * Renvoit le code javascript qui va déclancher l'evenement eventType sur l'élément pointé par le Xpath sous forme de string
         * Ce string permet de déclancher des évenements sur la page. On utilise cette méthode pour déclancher des clicks et over.
         * Selenium offre cette possibilité nativement mais elle n'est pas fiable à 100% contrairement à cette méthode.
         */
        public static string getEvent(string elementXpath, string eventType) {

            string str = "var event = new MouseEvent(" + eventType + ", {" +
                "'view': window," +
                "'bubbles': true," +
                "'cancelable': true" +
                "});" +
                "var myTarget = document.evaluate('" + elementXpath + "', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;" +
                "var canceled = !myTarget.dispatchEvent(event);";

            return str;
        }

    }
}