using System;
using System.Collections;
using System.IO;

namespace MyProduct
{
    /// <summary>
    /// Text File IO
    /// </summary>
    public class IoTextFile
    {
        private StreamWriter _writer = null;
        private readonly ArrayList _tempString = new ArrayList();
        private DirectoryInfo _currDir = new DirectoryInfo(".");

        ~IoTextFile() { }
        // Searching Current Directory
        /// <summary>
        /// Re-new the existing file (re-create)
        /// </summary>
        /// <param name="fileName">"File Name.txt"</param>
        public void CreateFileInDirectory(string dirpath)
        {
            try
            {
                _writer = File.CreateText(@dirpath);
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException("Cannot Create file! " + e.Message);
            }
            finally
            {
                _writer.Close();
            }
        }

        /// <summary>
        /// Overwrite existing Text File
        /// </summary>
        /// <param name="dirpath">E.g. C:\CSharpLearner\Day1</param>
        /// <param name="contain">ArrayList Format</param>
        /// 
        public void CreateWrite_TextFile(string dirpath, ArrayList contain)
        {
            try
            {
                if (!File.Exists(@dirpath))
                    CreateFileInDirectory(@dirpath);

                using (StreamWriter writer = File.CreateText(@dirpath))
                {
                    for (int i = 0; i < contain.Count; i++)
                    {
                        writer.WriteLine(contain[i]);
                    }
                    writer.Close();
                }
            }
            catch (FileNotFoundException)
            {

                throw new FileNotFoundException("Cannot Write Existing file!");
            }
        }
        public void CreateWriteToTextFile(string dirpath, ArrayList contain)
        {
            try
            {
                if (!File.Exists(@dirpath))
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                else
                {
                    using (StreamWriter writer = File.CreateText(@dirpath))
                    {
                        for (int i = 0; i < contain.Count; i++)
                        {
                            writer.WriteLine(contain[i]);
                        }
                        writer.Close();
                    }
                }
            }
            catch (FileNotFoundException)
            {

                throw new FileNotFoundException("Cannot Write Existing file!");
            }
        }
        /// <summary>
        /// Log New Data and append with new line into Existing File
        /// </summary>
        /// <param name="dirpath">E.g. C:\CSharpLearner\Day1</param>
        /// <param name="contain">E.g. new string[] { "New", "New File" }</param>
        public void WriteNewLineToExistTextFile(string dirpath, string[] contain)
        {
            try
            {
                if (!File.Exists(@dirpath))
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                else
                {
                    using (StreamWriter writer = File.AppendText(@dirpath))
                    {
                        for (int i = 0; i < contain.Length; i++)
                        {
                            writer.Write(contain[i] + ",");
                        }
                        writer.WriteLine();
                        writer.Close();
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException("Cannot Write/Append the file!\n" + ex.Message);
            }
        }
        /// <summary>
        /// Log New Data and append with new line into Existing File
        /// </summary>
        /// <param name="dirpath">E.g. C:\CSharpLearner\Day1</param>
        /// <param name="contain">contain in string format</param>
        public void WriteNewLineToExistTextFile(string dirpath, string contain)
        {
            try
            {
                if (!File.Exists(@dirpath))
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                else
                {
                    using (StreamWriter writer = File.AppendText(@dirpath))
                    {                        
                        writer.Write(contain);
                        writer.WriteLine();
                        writer.Close();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Cannot Write/Append the file!");
            }
        }
        /// <summary>
        /// Upgrate Data of Esxiting text File
        /// </summary>
        /// <param name="dirpath">"Setup.txt"</param>
        /// <param name="groupName">E.g. Global, Spec which is match with existing text file</param>
        /// <param name="targetUpgradeName">E.g. max, min which is match with existing text file contain</param>
        /// <param name="upgradeData">String Format, which is going to upgrate the data in existing text file</param>
        public void WriteUpgradeDataToExistTestFile(string dirpath, string groupName,
            string targetUpgradeName, string upgradeData)
        {
            ArrayList localTextList = new ArrayList();
            ArrayList tempTextList = new ArrayList();

            localTextList = ReadTextFile(dirpath);
            for (int i = 0; i < localTextList.Count; i++)
            {
                if (localTextList[i].ToString() == "[" + groupName + "]")
                {

                    char[] temp = { };
                    string[] templine;
                    i++;
                    try
                    {
                        while (localTextList[i].ToString() != null)
                        {
                            temp = localTextList[i].ToString().ToCharArray();
                            templine = localTextList[i].ToString()
                                .Split(new char[] { ' ', '=', ' ' });

                            if (temp[0] == '[' && temp[temp.Length - 1] == ']')
                                break;

                            if (templine[0] == targetUpgradeName)
                            {
                                templine[templine.Length - 1] = " = " + upgradeData;
                                localTextList[i] = string.Join("", templine);
                                break;
                            }
                            i++;
                        }
                        break;
                    }
                    catch (FileNotFoundException)
                    {

                        throw new FileNotFoundException("Cannot Find " + targetUpgradeName);
                    }
                }
            }

            CreateWriteToTextFile(dirpath, localTextList);
        }

        public ArrayList ReadCalFile(string dirpath)
        {
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        _tempString.Clear();

                        while ((line = reader.ReadLine()) != null)
                        {
                            string templine = line.ToString();
                            char[] myChars = templine.ToCharArray();
                            if (myChars[0] != '#')
                            {
                                _tempString.Add(line.ToString());
                            }
                        }
                        reader.Close();
                    }
                }
                return _tempString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " Cannot Read from the file!");
            }
        }
        /// <summary>
        /// Read all dtaa from text file, Note: Return as ArrayList Format
        /// </summary>
        /// <param name="dirpath">"Setup.txt"</param>
        /// <returns></returns>
        public ArrayList ReadTextFile(string dirpath)
        {
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        _tempString.Clear();

                        while ((line = reader.ReadLine()) != null)
                        {
                            _tempString.Add(line.ToString());
                        }
                        reader.Close();
                    }
                }
                return _tempString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " Cannot Read from the file!");
            }
        }
        /// <summary>
        /// Read section Data [ ] from Text file, Note: Return as ArrayList Format
        /// </summary>
        /// <param name="dirpath">"Setup.txt"</param>
        /// <param name="groupName">E.g. Global, Spec which is match with existing text file</param>
        /// <returns>return as ArrayList format</returns>
        public ArrayList ReadTextFile(string dirpath, string groupName)
        {
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        _tempString.Clear();

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == "[" + groupName + "]")
                            {
                                char[] temp = { };
                                line = reader.ReadLine();
                                while (line != null && line != "")
                                {
                                    temp = line.ToCharArray();
                                    if (temp[0] == '[' && temp[temp.Length - 1] == ']')
                                        break;
                                    _tempString.Add(line.ToString());
                                    line = reader.ReadLine();
                                }
                                break;
                            }
                        }
                        reader.Close();
                    }
                }
                return _tempString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " " + groupName
                    + " Cannot Read from the file!");
            }
        }
        /// <summary>
        /// Read section Data [ ] from Text file, Note: Return as String Format
        /// </summary>
        /// <param name="dirpath">"Setup.txt"</param>
        /// <param name="groupName">E.g. Global, Spec which is match with existing text file</param>
        /// <param name="targetName">E.g. Global.NPLC, Spec.VoltageAcc.Offset</param>
        /// <returns>return as String format</returns>
        public String ReadTextFile(string dirpath, string groupName, string targetName)
        {
            string tempSingleString;
            try
            {
                if (!File.Exists(@dirpath))
                {
                    throw new FileNotFoundException("{0} does not exist."
                        , @dirpath);
                }
                else
                {
                    using (StreamReader reader = File.OpenText(@dirpath))
                    {
                        string line = "";
                        string[] templine;
                        tempSingleString = "";

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line == "[" + groupName + "]")
                            {
                                char[] temp = { };
                                line = reader.ReadLine();
                                while (line != null && line != "")
                                {
                                    templine = line.ToString()
                                        .Split(new char[] {'='});
                                    temp = line.ToCharArray();
                                    if (temp[0] == '[' && temp[temp.Length - 1] == ']')
                                        break;
                                    if (templine[0].TrimEnd() == targetName)
                                    {
                                        tempSingleString= templine[templine.Length - 1].ToString().TrimStart();
                                        break;
                                    }
                                    line = reader.ReadLine();
                                }
                                break;
                            }
                        }
                        reader.Close();
                    }
                }
                return tempSingleString;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(dirpath + " " + groupName + " " +
                    targetName + " Cannot Read from the file!");
            }
        }

        private void RunCmd(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "CMD.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public void CompressSdiFileInDirectory(string defaultPath, string folderName)
        {
            RunCmd("/C cd " + defaultPath + " && tar -cvf " + folderName + ".tar " + folderName);
            RunCmd("/C cd " + defaultPath + " && bzip2 " + folderName + ".tar");
        }

        public void CopyFile(string source, string target)
        {
            File.Copy(source, target);
        }

        public double Grab_CalData(string dirpath, string queryType, double startFreq, double stopFreq , string rtnData)
        {
            int count;
            int startFreqCounter;
            bool startFreqStatus;
            int stopFreqCounter;
            bool stopFreqStatus;
            bool sameStartStopFreq;
            string[] tmpArr;
            double[] freqArray;
            double[] siggenArray;
            double[] pathlossArray;
            double[] measurementArray;
            double lossData;

            ArrayList localTextList = new ArrayList();
            ArrayList tempTextList = new ArrayList();

            //Initialize variable
            count = 0;
            startFreqCounter = 0;
            startFreqStatus = false;
            stopFreqCounter = 0;
            stopFreqStatus = false;
            sameStartStopFreq = false;
            lossData = 99999;

            //remove line with # and blank space from existing cal data
            tempTextList = ReadCalFile(dirpath);

            //sort the arraylist into respective variable 
            freqArray = new double[tempTextList.Count];
            measurementArray = new double[tempTextList.Count];
            siggenArray = new double[tempTextList.Count];
            pathlossArray = new double[tempTextList.Count];
            
            tmpArr = new string[4];

            for (count = 0; count < tempTextList.Count; count++)
            {
                tmpArr = tempTextList[count].ToString().Split(',');
                freqArray[count] = Convert.ToDouble(tmpArr[0]);
                measurementArray[count] = Convert.ToDouble(tmpArr[1]);
                siggenArray[count] = Convert.ToDouble(tmpArr[2]);
                pathlossArray[count] = Convert.ToDouble(tmpArr[3]);
            }

            //get start & stop cal data base on freq input
            if (stopFreq > startFreq)
            {
                for (count = 0; count < tempTextList.Count - 1; count++)
                {  
                    if (startFreq >= freqArray[count] && startFreq < freqArray[count + 1])
                    {
                        startFreqCounter = count;
                        startFreqStatus = true;
                        break;
                    }
                    else if (startFreq > freqArray[count] && startFreq <= freqArray[count + 1])
                    {
                        startFreqCounter = count + 1;
                        startFreqStatus = true;
                        break;
                    }
                }

                for (count = 1; count < tempTextList.Count; count++)
                {
                    if (stopFreq >= freqArray[count - 1] && stopFreq < freqArray[count])
                    {
                        stopFreqCounter = count - 1 ;
                        stopFreqStatus = true;
                        break;
                    }
                    else if (stopFreq > freqArray[count - 1] && stopFreq <= freqArray[count])
                    {
                        stopFreqCounter = count;
                        stopFreqStatus = true;
                        break;
                    }
                }
            }
            else if (startFreq == stopFreq) //If StartFreq and StopFreq define within same freq interval, set Same_StartStopFreq status = true
            {
                for (count = 0; count < tempTextList.Count - 1; count++)
                {
                    if (startFreq >= freqArray[count] && startFreq < freqArray[count + 1])
                    {
                        startFreqCounter = count;
                        stopFreqCounter = count;
                        sameStartStopFreq = true;
                        queryType = "FIXED";        //Force queryType to be FIXED because start and stop freq are equal thus not required search
                        break;
                    }
                    else if (startFreq > freqArray[count] && startFreq <= freqArray[count + 1])
                    {
                        startFreqCounter = count + 1;
                        stopFreqCounter = count + 1;
                        sameStartStopFreq = true;
                        queryType = "FIXED";        //Force queryType to be FIXED because start and stop freq are equal thus not required search
                        break;
                    }
                }
            }
            else
            {
               lossData = 99999; //error encounter - return big number
            }

            switch (queryType.ToUpper())
            {
                case "AVE":
                    double totalLoss;
                    totalLoss = 0;
                    for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                    {
                        if (rtnData.ToUpper() == "ACTUAL_MEASUREMENT")  //return data from 'ACTUAL_MEASUREMENT' column
                        {
                            totalLoss = totalLoss + measurementArray[j];
                        }
                        else if (rtnData.ToUpper() == "LOSS_FACTOR")  //return data from 'LOSS_FACTOR' column
                        {
                            totalLoss = totalLoss + pathlossArray[j];
                        }
                        else if (rtnData.ToUpper() == "SIGGEN_AMPLITUDE")  //return data from 'SIGGEN_AMPLITUDE' column
                        {
                            totalLoss = totalLoss + siggenArray[j];
                        }
                    }
                    //Check if startfreq and stopfreq is within caldata file range
                    if (startFreqStatus == true && stopFreqStatus == true)
                    {
                        lossData = totalLoss/((stopFreqCounter - startFreqCounter) + 1);  //Average will always divide by 1 because start and stop freq are same
                    }
                    else
                    {
                        lossData = 99999; //error encounter - return big number
                    }                 
                    break;
                case "MAX":
                    //Check if startfreq and stopfreq is within caldata file range
                    if (startFreqStatus == true && stopFreqStatus == true)
                    {
                        if (rtnData.ToUpper() == "ACTUAL_MEASUREMENT")  //return data from 'ACTUAL_MEASUREMENT' column
                        {
                            lossData = measurementArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData < measurementArray[j])
                                {
                                    lossData = measurementArray[j];
                                }
                            }
                        }
                        else if (rtnData.ToUpper() == "LOSS_FACTOR")     //return data from 'LOSS_FACTOR' column
                        {
                            lossData = pathlossArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData < pathlossArray[j])
                                {
                                    lossData = pathlossArray[j];
                                }
                            }
                        }
                        else if (rtnData.ToUpper() == "SIGGEN_AMPLITUDE")     //return data from 'SIGGEN_AMPLITUDE' column
                        {
                            lossData = siggenArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData < siggenArray[j])
                                {
                                    lossData = siggenArray[j];
                                }
                            }
                        }
                    }
                    else
                    {
                        lossData = 99999; //error encounter - return big number
                    }                    
                    break;
                case "MIN":
                //Check if startfreq and stopfreq is within caldata file range
                    if (startFreqStatus == true && stopFreqStatus == true)
                    {
                        if (rtnData.ToUpper() == "ACTUAL_MEASUREMENT")  //return data from 'ACTUAL_MEASUREMENT' column
                        {
                            lossData = measurementArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData > measurementArray[j])
                                {
                                    lossData = measurementArray[j];
                                }
                            }
                        }
                        else if (rtnData.ToUpper() == "LOSS_FACTOR")  //return data from 'LOSS_FACTOR' column
                        {
                            lossData = pathlossArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData > pathlossArray[j])
                                {
                                    lossData = pathlossArray[j];
                                }
                            }
                        }
                        else if (rtnData.ToUpper() == "SIGGEN_AMPLITUDE")  //return data from 'SIGGEN_AMPLITUDE' column
                        {
                            lossData = siggenArray[startFreqCounter];
                            for (int j = startFreqCounter; j <= stopFreqCounter; j++)
                            {
                                if (lossData > siggenArray[j])
                                {
                                    lossData = siggenArray[j];
                                }
                            }
                        }
                    }
                    else
                    {
                        lossData = 99999; //error encounter - return big number
                    }        
                    break;
                case "FIXED":
                //Check if startfreq and stopfreq is same. Note: for "FIXED" both start & stop freq must be same
                    if (sameStartStopFreq == true)
                    {
                        if (rtnData.ToUpper() == "ACTUAL_MEASUREMENT")
                        {
                            lossData = measurementArray[startFreqCounter];
                        }
                        else if (rtnData.ToUpper() == "LOSS_FACTOR")  //return data from 'LOSS_FACTOR' column
                        {
                            lossData = pathlossArray[startFreqCounter];
                        }
                        else if (rtnData.ToUpper() == "SIGGEN_AMPLITUDE")  //return data from 'SIGGEN_AMPLITUDE' column
                        {
                            lossData = siggenArray[startFreqCounter];
                        }
                    }
                    else
                    {
                        lossData = 99999; //error encounter - return big number
                    }
                    break;
                default:
                    lossData = 99999; //error encounter - return big number
                    break;
            }

            return lossData;
        }
    }
}
