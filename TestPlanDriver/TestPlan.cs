
#region COMMNET and Copyright SECTION (NO MANUAL TOUCH!)
// This is AUTOMATIC generated template Test Plan (.cs) file for ATF (Clotho) of WSD, AvagoTech: V2.2.1.0
// Any Questions or Comments, Please Contact: YU HAN, yu.han@avagotech.com
// NOTE 1: Test Plan template .cs has 'FIXED' Sections which should NEVER be Manually Touched 
// NOTE 2: Starting from V2.2.0, Clotho follows new Package style test plan management:
//       (a) Requires valid integer Version defined for TestPlan, TestLimit, and ExcelUI
//               For TestPlan.cs, refer to header item 'TestPlanVersion=1'
//               For TestLiimit.csv, refer to row #7 'SpecVersion,1'
//               For ExcelUI.xlsx, refer to sheet #1, row #1 'VER	1'
//       Note TestPlanTemplateGenerator generated items holds default version as '1'
//       (b) About ExcelUI file and TestLimit file:
//               Always load from same parent folder as Test Plan .cs, @ root level
//       (c) About Correlation File:
//               When Development mode, loaded from  C:\Avago.ATF.Common.x64\CorrelationFiles\Development\
//               When Production mode, loaded from package folder within C:\Avago.ATF.Common.x64\CorrelationFiles\Production\
#endregion COMMNET and Copyright SECTION

#region Test Plan Properties Section (NO MANUAL TOUCH)
////<TestPlanVersion>TestPlanVersion=1<TestPlanVersion/>
////<ExcelBuddyConfig>BuddyExcel = \TCF\BONSAI_Rev6.0_Cond.xlsx;ExcelDisplay = 1<ExcelBuddyConfig/>
////<xTestLimitBuddyConfig>BuddyTestLimit = \TSF\BLACKBEAD_Limit_ZnBT_4port_Rev5.2.csv<TestLimitBuddyConfig/>
////<xCorrelationBuddyConfig>BuddyCorrelaton = BLACKBEAD_Corr_ZnBT_4port_Rev5.2.csv<CorrelationBuddyConfig/>
#endregion Test Plan Properties Section


#region Test Plan Hardware Configuration Section
#endregion Test Plan Hardware Configuration Section


#region Test Plan Parameters Section
////<TestParameter>Name="SimHW";Type="IntType";Unit=""<TestParameter/>
#endregion Test Plan Parameters Section


#region Singel Value Parameters Section
////<SingelValueParameter>Name="SimHW";Value="1";Type="IntType";Unit=""<SingelValueParameter/>
#endregion Singel Value Parameters Section


#region Test Plan Sweep Control Section (NO MANUAL TOUCH!)
#endregion Test Plan Sweep Control Section


#region 'FIXED' Reference Section (NO MANUAL TOUCH!)
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Ivi.Visa.Interop;

using Avago.ATF.StandardLibrary;
using Avago.ATF.Shares;
using Avago.ATF.Logger;
using Avago.ATF.LogService;
#endregion 'FIXED' Reference Section


#region Custom Reference Section
//////////////////////////////////////////////////////////////////////////////////
// ----------- ONLY provide your Custom Reference 'Usings' here --------------- //
using MyProduct;
using System.Net;

using clsfrmInputUI;
using ProductionControl_x86;
using AvagoGU;

// ----------- END of Custom Reference 'Usings' --------------- //
//////////////////////////////////////////////////////////////////////////////////
#endregion Custom Reference Section


public class TestPlan : MarshalByRefObject, IATFTest
{
    MyDutFbar _myDut;
    GU.UI ICCCAL_UI = GU.UI.Production;

    #region  SNP (Datalog) variable
    IPHostEntry _ipEntry = null;
    DateTime _dt = new DateTime();

    string
        _tPVersion = "",
    _productTag = "",
    _lotId = "",
    _sublotId = "",
    _waferId = "",
    _opId = "",
    _handlerSn = "",
    _newPath = "",
    _fileName = "",
    _testerHostName = "",
    _testerIp = "";

    readonly string
    _activeDir = @"C:\Avago.ATF.Common\DataLog\";

    //Temp string for current Lot and SubLot ID - to solve Inari issue when using Tally Generator without unload testplan
    //This will cause the datalog for current lot been copied to previous lot folder
    string _previousLotSubLotId = "";

    readonly string _currentLotSubLotId = "";

    string _tempWaferId = "",
        _tempOpId = "",
        _tempHandlerSn = "";

    #endregion

    //GUI ENTRY Variable flag
    bool _guiEnable = false;

    string _initProTag = "";
    bool _firstTest;
    bool _programLoadSuccess = true;

    public string DoATFInit(string args)
    {
        Debugger.Break();

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Enter DoATFInit: {0}\nDo Minimum HW Init:\n{1}\n", args, ATFInitializer.DoMinimumHWInit());


        #region Custom Init Coding Section
        //////////////////////////////////////////////////////////////////////////////////
        // ----------- ONLY provide your Custom Init Coding here --------------- //
        
        _myDut = new MyDutFbar(ref sb) {TmpUnitNo = 0};
        _programLoadSuccess = _myDut.InitSuccess;

        _initProTag = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_PACKAGE_TAG, "");
        Clotho.EnableClothoTextBoxes(false);
        _firstTest = true;

        //Check boolean status of GUI ENTRY
        _guiEnable = false;

        FrmDataInput formInput;
        if (_guiEnable == true)
        {
            #region New InputUI
            formInput = new FrmDataInput();

            //string AssemblyID_ = " ";

            DialogResult rslt = formInput.ShowDialog();

            if (rslt == DialogResult.OK)
            {
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_OP_ID, formInput.OperatorId + "\t");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_LOT_ID, formInput.LotId + "\t");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_SUB_LOT_ID, formInput.SublotId + "\t");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_DIB_ID, formInput.LoadBoardId + "\t");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_CONTACTOR_ID, formInput.ContactorId + "\t");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_HANDLER_SN, formInput.HandlerId);
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_PCB_ID, "NA");
                ATFCrossDomainWrapper.StoreStringToCache(PublishTags.PUBTAG_WAFER_ID, "NA");

            }

            #region Lock ClothoUI
            if (!formInput.AdminLevel)
            {
                Thread t1 = new Thread(new ThreadStart(LockClothoInputUi));
                t1.Start();
            }
            #endregion

            #endregion
        }

        
        GU.DoInit_afterCustomCode(true, false, _myDut.ProductTag, @"C:\Avago.ATF.Common\Results");

        // ----------- END of Custom Init Coding --------------- //
        //////////////////////////////////////////////////////////////////////////////////
        #endregion Custom Init Coding Section

        return sb.ToString();
    }


    public string DoATFUnInit(string args)
    {
        Debugger.Break();

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Enter DoATFUnInit: {0}\n", args);


        #region Custom UnInit Coding Section
        //////////////////////////////////////////////////////////////////////////////////
        // ----------- ONLY provide your Custom UnInit Coding here --------------- //

        _myDut.UnInit();
        GU.forceReload = false;
        // ----------- END of Custom UnInit Coding --------------- //
        //////////////////////////////////////////////////////////////////////////////////
        #endregion Custom UnInit Coding Section

        return sb.ToString();
    }


    public string DoATFLot(string args)
    {
        Debugger.Break();

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Enter DoATFLot: {0}\n", args);


        #region Custom CloseLot Coding Section
        //////////////////////////////////////////////////////////////////////////////////
        // ----------- ONLY provide your Custom CloseLot Coding here --------------- //




        // ----------- END of Custom CloseLot Coding --------------- //
        //////////////////////////////////////////////////////////////////////////////////
        #endregion Custom CloseLot Coding Section

        return sb.ToString();
    }


    public ATFReturnResult DoATFTest(string args)
    {
        //Debugger.Break();

        string err = "";
        StringBuilder sb = new StringBuilder();
        ATFReturnResult result = new ATFReturnResult();

        // ----------- Example for Argument Parsing --------------- //
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (!ArgParser.parseArgString(args, ref dict))
        {
            err = "Invalid Argument String" + args;
            MessageBox.Show(err, "Exit Test Plan Run", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new ATFReturnResult(err);
        }


        int simHw;
        try
        {
            simHw = ArgParser.getIntItem(ArgParser.TagSimMode, dict);
        }
        catch (Exception ex)
        {
            err = ex.Message;
            MessageBox.Show(err, "Exit Test Plan Run", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return new ATFReturnResult(err);
        }
        // ----------- END of Argument Parsing Example --------------- //


        #region Custom Test Coding Section
        //////////////////////////////////////////////////////////////////////////////////
        // ----------- ONLY provide your Custom Test Coding here --------------- //
        // Example for build TestPlan Result (Single Site)

        if (_firstTest == true)
        {
            string[] resultFileName = ATFCrossDomainWrapper.GetClothoCurrentResultFileFullPath().Split('_');

            if (_guiEnable == true)
            {
                if (resultFileName[0] != _initProTag)
                {
                    _programLoadSuccess = false;
                    MessageBox.Show("Product Tag accidentally changed to: " + resultFileName[0] + "\nPlease re-load program!");
                    err = "Product Tag accidentally changed to: " + resultFileName[0];
                    return new ATFReturnResult(err); ;
                }
            }
        }

        if (!_programLoadSuccess)
        {
            MessageBox.Show("Program was not loaded successfully.\nPlease resolve errors and reload program.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            err = "Program was not loaded successfully.\nPlease resolve errors and reload program";
            return new ATFReturnResult(err); 
        }

        GU.DoTest_beforeCustomCode();

        #region Retrieve lot ID# (for Datalog)
        //Retrieve lot ID#
        _tPVersion = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_PACKAGE_TP_VER, "");
        _productTag = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_PACKAGE_TAG, "").ToUpper();
        _lotId = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_LOT_ID, "").ToUpper();
        _sublotId = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_SUB_LOT_ID, "").ToUpper();
        _waferId = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_WAFER_ID, "");
        _opId = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_OP_ID, "");
        _handlerSn = ATFCrossDomainWrapper.GetStringFromCache(PublishTags.PUBTAG_HANDLER_SN, "");
        _testerHostName = System.Net.Dns.GetHostName();
        _ipEntry = System.Net.Dns.GetHostEntry(_testerHostName);
        _testerIp = NetworkHelper.GetStaticIPAddress().Replace(".", "");      //Use Clotho method , original code has issue with IPv6 - 12/03/2015 Shaz

        if (_myDut.TmpUnitNo == 0)      //do this for the 1st unit only
        {
            _dt = DateTime.Now;

            if (_productTag != "" )
            {            
                _newPath = System.IO.Path.Combine(_activeDir, _productTag + "_" + _lotId + "_" + _sublotId + "_" + _testerIp + "\\");

            }
            else
            {
                string tempname = "DebugMode_" + _dt.ToString("yyyyMMdd" + "_" + "HHmmss");
                _newPath = System.IO.Path.Combine(_activeDir, tempname + "\\");
                _productTag = "Debug";
                _lotId = "1";
            }

            //Parse information to LibFbar
            _myDut.SnpFile.FileSourcePath = _newPath;
            _myDut.SnpFile.FileOutputFileName = _productTag;
            _myDut.SnpFile.LotID = _lotId;
            // Added variable to solve issue with datalog when Inari operator using 
            //Tally Generator to close lot instead of unload test plan
            //WaferId,OpId and HandlerSN are null when 2nd Lot started - make assumption that this 3 param are similar 1st Lot
            _tempWaferId = _waferId;
            _tempOpId = _opId;
            _tempHandlerSn = _handlerSn;
            _previousLotSubLotId = _currentLotSubLotId;
        }

        DateTime DT = DateTime.Now;
        List<string> FileContains = new List<string>();
        FileContains.Add("DATE_START=" + DT.ToString("yyyyMMdd"));
        FileContains.Add("TIME_START=" + DT.ToString("HHmmss"));
        FileContains.Add("ENTITY=NA");
        FileContains.Add("SOURCE=FBAR");
        FileContains.Add("GROUP=DUPLEXER");
        FileContains.Add("PRODUCT_TAG=" + _productTag);
        FileContains.Add("LOT_NUMBER=" + _lotId + "_" + _sublotId);
        FileContains.Add("WAFER_ID=" + _waferId );
        FileContains.Add("OPERATOR_NAME=" + _opId);
        FileContains.Add("TESTER_NAME=FBAR");
        FileContains.Add("TESTER_HOST_NAME=" + _testerHostName);
        FileContains.Add("HANLDER_NAME=" + _handlerSn);
        FileContains.Add("TEST_PLAN_VERSION=" + _tPVersion);

        _myDut.FileFileContain = FileContains;
        #endregion

#if (!DEBUG)
    _myDut.TmpUnitNo = Convert.ToInt32(ATFCrossDomainWrapper.GetClothoCurrentSN());
#else
        _myDut.TmpUnitNo++;      // Need to enable this during debug mode
#endif

        for (GU.runLoop = 1; GU.runLoop <= GU.numRunLoops; GU.runLoop++)
        {
            ATFResultBuilder.Reset();
            _firstTest = false;
            _myDut.RunTest(ref result);

            GU.DoTest_afterCustomCode(ref result);
        }

        // ----------- END of Custom Test Coding --------------- //
        //////////////////////////////////////////////////////////////////////////////////
        #endregion Custom Test Coding Section

        //ATFReturnResult result = new ATFReturnResult();
        //ATFResultBuilder.AddResult(ref result, "PARAM", "X", 0.01);
        return result;
    }

    private void LockClothoInputUi()
    {
        Clotho.EnableClothoTextBoxes(false);
        Thread.Sleep(5);
        Clotho.EnableClothoTextBoxes(false);
        Thread.Sleep(10);
        Clotho.EnableClothoTextBoxes(false);
        Thread.Sleep(15);
        Clotho.EnableClothoTextBoxes(false);
        Thread.Sleep(20);
        Clotho.EnableClothoTextBoxes(false);
    }

}
