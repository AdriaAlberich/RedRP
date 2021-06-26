/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace redrp
{
    /// <summary>
    /// Admin reports system
    /// </summary>
    public class AdminReport : Script
    {

        // REPORT ATTRIBUTES

        /// <summary>
        /// Report identifier
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Player who sent the report
        /// </summary>
        public Player reporter { get; set; }

        /// <summary>
        /// Admin who has taken care of the report
        /// </summary>
        public Player admin { get; set; }

        /// <summary>
        /// Report text
        /// </summary>
        public string reportText { get; set; }

        /// <summary>
        /// Time elapsed since creation
        /// </summary>
        public int timeElapsed { get; set; }

        /// <summary>
        /// Void constructor
        /// </summary>
        public AdminReport()
        {

        }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="reporter">Player who sends the report</param>
        /// <param name="reportText">The report text</param>
        public AdminReport(Player reporter, string reportText)
        {
            this.id = GetFirstAvailableId();
            this.reporter = reporter;
            this.reportText = reportText;
            this.admin = null;
            this.timeElapsed = 0;

            Global.AdminReports.Add(this);
        }

        /// <summary>
        /// Gets the first available id
        /// </summary>
        /// <returns></returns>
        public static int GetFirstAvailableId()
        {
            int availableId = 0;

            for(int id = 1; id < Global.MaxAdminReports; id++)
            {
                foreach (AdminReport report in Global.AdminReports)
                {
                    if (report.id != id)
                    {
                        availableId = id;
                    }
                }

                if(availableId == 0)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            
            return availableId;
        }

        /// <summary>
        /// Generates the admin report menu list
        /// </summary>
        /// <param name="player">The player (admin)</param>
        public static void GenerateReportsMenu(Player player)
        {
            Menu menu = new Menu("", "Reportes", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnReportSelected));
            menu.miscData = new List<int>();

            foreach (AdminReport report in Global.AdminReports)
            {
                MenuItemModel reportModel = new MenuItemModel();
                if (report.admin != null)
                {
                    string attendedBy = report.admin.name;
                    reportModel.itemDescription = "Atendido por " + attendedBy;
                }
                else
                {
                    reportModel.itemDescription = "Pendiente de atender";
                }
                 
                reportModel.itemName = "~s~[#" + report.id + "] | " + report.reporter.character.cleanName + " (" + report.reporter.id + ") | " + report.reportText + " | " + report.timeElapsed + " segundos.";
                menu.miscData.Add(report.id);
            }

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Manages report menu responses
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="option">The selected option as string</param>
        /// <param name="actionId">The selected option</param>
        public static void OnReportSelected(Player player, string option, int actionId)
        {
            AdminReport report = Global.AdminReports[actionId];
            if (report != null)
            {
                if (report.admin == null)
                {
                    report.admin = player;
                    Admin.Notification(player.name + " ha atendido el reporte de " + report.reporter.character.cleanName + " (" + report.reporter.id + ").");
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~b~Reporte: " + report.reportText);
                }
                else
                {
                    Global.AdminReports.Remove(report);
                    player.DisplayNotification("~g~Reporte eliminado.");
                }
            }
            else
            {
                player.DisplayNotification("~g~Este reporte ya fue eliminado.");
            }

            GenerateReportsMenu(player);
        }

        /// <summary>
        /// Manages report dialog responses
        /// </summary>
        /// <param name="sender">The player who sends the report</param>
        /// <param name="reportText">The report text</param>
        public static void OnReportSent(Player sender, string reportText)
        {
            if (reportText.Trim().Length > 2)
            {
                if (Global.AdminReports.Count < Global.MaxAdminReports)
                {
                    if (reportText.Trim().Length <= 64)
                    {
                        AdminReport report = new AdminReport(sender, reportText.Trim());
                        Admin.Notification("Nuevo reporte de " + sender.character.cleanName + " (" + sender.id + "), /atender " + report.id + ".");
                        sender.DisplayNotification("~b~Reporte enviado, espera a que te atiendan.");
                    }
                    else
                    {
                        sender.DisplayNotification("~r~Reporte demasiado largo (máximo 64 carácteres).");
                    }
                }
                else
                {
                    sender.DisplayNotification("~r~Bandeja de reportes llena, espera que se vacíen.");
                }
            }
            else
            {
                sender.DisplayNotification("~r~Elabora tu reporte porfavor.");
            }
        }

        /// <summary>
        /// Returns true if player has an open report
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <returns>True if has an open report, false otherwise</returns>
        public static bool HasReport(Player player)
        {
            bool hasReport = false;
            foreach(AdminReport report in Global.AdminReports)
            {
                if(report.reporter.Equals(player))
                {
                    hasReport = true;
                }
            }

            return hasReport;
        }

        /// <summary>
        /// Gets the report instance by its id
        /// </summary>
        /// <param name="id">The report id</param>
        /// <returns>The report instance or null if not found</returns>
        public static AdminReport GetById(int id)
        {
            AdminReport reportFound = null;
            foreach (AdminReport report in Global.AdminReports)
            {
                if (report.id == id)
                {
                    reportFound = report;
                }
            }

            return reportFound;
        }
    }

}
