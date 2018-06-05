using System;

namespace SparamTestLib
{
    public class SparamDelta : SparamTestCase.TestCaseAbstract
    {
        int val_1, val_2;

        public override void Initialize()
        {
            _Result = new SResult();

            try
            {
                string[] tmpstr = UsePrevious.Split(',');
                val_1 = int.Parse(tmpstr[0]);
                val_2 = int.Parse(tmpstr[1]);
            }
            catch
            {
                DisplayError("SparamDelta", "Initialize - wrong input " + UsePrevious , "Use_Previous column data must in this format - (2,6)");
            }

            Array.Resize(ref _Result.Result, 1);
            Array.Resize(ref _Result.Header, 1);

            _Result.Enable = true;
            _Result.Header[0] = TcfHeader + "_" + tmpResult[val_1].Header[0] + "_" + tmpResult[val_2].Header[0];
            _Result.Result[0] = -9999;
        }
        public override void RunTest()
        {
            double rtnVal;

            rtnVal = tmpResult[val_1].Result[0] - tmpResult[val_2].Result[0];

            if (Abs)
                rtnVal = Math.Abs(rtnVal);

            _Result.Result[0] = rtnVal;
        }
    }
}
