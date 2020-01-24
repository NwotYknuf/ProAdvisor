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
         * Renvoit le code javascript qui va déclancher l'evenement eventType sur l'élément pointé par le Xpath
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