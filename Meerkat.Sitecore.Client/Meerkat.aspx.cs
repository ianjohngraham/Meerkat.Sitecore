using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Meerkat.Sitecore
{
    public partial class Meerkat : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var file = Request.QueryString["zipfile"];
            if (!string.IsNullOrEmpty(file))
            {
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + file.Substring((file.LastIndexOf("/") +1), file.Length - (file.LastIndexOf("/") + 1)));
                Response.WriteFile(file);
                Response.End();
                Response.Close();
            }
        }
    }
}

