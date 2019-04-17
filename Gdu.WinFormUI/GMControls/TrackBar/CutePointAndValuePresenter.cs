/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;

namespace Gdu.WinFormUI
{
    public class CutePointAndValuePresenter
    {
        #region 内部变量

        int eggCount = 10;
        int markCount = 10;
        bool fcp;

        double averageEggsInEachGab;
        int firstMarkN;
        int lastMarkN;

        #endregion

        #region 构造函数

        public CutePointAndValuePresenter()
        {
            UpdateEggAndMarkInfo();
        }

        #endregion

        #region 公开的属性，方法

        public int PointCount
        {
            get
            {
                return fcp ? eggCount : markCount;
            }
        }

        public int ValueCount
        {
            get
            {
                return fcp ? markCount : eggCount;
            }
        }

        public void SetPointAndValueCount(int pointCount, int valueCount)
        {
            if (pointCount < 1)
                pointCount = 1;
            if (valueCount < 1)
                valueCount = 1;
            if (pointCount >= valueCount)
            {
                fcp = true;
                eggCount = pointCount;
                markCount = valueCount;
            }
            else
            {
                fcp = false;
                eggCount = valueCount;
                markCount = pointCount;
            }
            UpdateEggAndMarkInfo();
        }

        public void GetPointIndexFromValueIndex(int vIndex, out int pIndexFrom, out int pIndexTo)
        {

            if (fcp)
            {
                if (vIndex < 0 || vIndex > markCount - 1)
                    throw new ArgumentOutOfRangeException("vIndex");

                GetEggsIndexFromMarkIndex(vIndex, out pIndexFrom, out pIndexTo);
            }
            else
            {
                if (vIndex < 0 || vIndex > eggCount - 1)
                    throw new ArgumentOutOfRangeException("vIndex");

                GetMarkIndexFromEggIndex(vIndex, out pIndexFrom);
                pIndexTo = pIndexFrom;
            }
        }

        public void GetValueIndexFromPointIndex(int pIndex, out int vIndexFrom, out int vIndexTo)
        {
            if (fcp)
            {
                if (pIndex < 0 || pIndex > eggCount - 1)
                    throw new ArgumentOutOfRangeException("pIndex");

                GetMarkIndexFromEggIndex(pIndex, out vIndexFrom);
                vIndexTo = vIndexFrom;
            }
            else
            {
                if (pIndex < 0 || pIndex > markCount - 1)
                    throw new ArgumentOutOfRangeException("pIndex");

                GetEggsIndexFromMarkIndex(pIndex, out vIndexFrom, out vIndexTo);
            }
        }

        public void GetExactPointIndexFromValueIndex(int vIndex, out int pIndex)
        {
            int pIndex2;
            GetPointIndexFromValueIndex(vIndex, out pIndex, out pIndex2);
            int maxIndex = fcp ? markCount - 1 : eggCount - 1;
            if (vIndex == maxIndex)
                pIndex = pIndex2;
            else if (vIndex != 0)
                pIndex = (pIndex + pIndex2) / 2;
        }

        public void GetExactValueIndexFromPointIndex(int pIndex, out int vIndex)
        {
            int vIndex2;
            GetValueIndexFromPointIndex(pIndex, out vIndex, out vIndex2);
            int maxIndex = fcp ? eggCount - 1 : markCount - 1;
            if (pIndex == maxIndex)
                vIndex = vIndex2;
            else if (pIndex != 0)
                vIndex = (vIndex + vIndex2) / 2;
        }

        #endregion

        #region 内部函数

        private int GetUpRoundedHalfValue(int a)
        {
            int t = a / 2;
            if (a % 2 != 0)
                t++;
            return t;
        }

        private int EggsCount(int markIndex)
        {
            return (markIndex + 1) +
                (int)Math.Round(averageEggsInEachGab * ((double)markIndex + 0.5), MidpointRounding.AwayFromZero);
        }

        private void UpdateEggAndMarkInfo()
        {
            if (markCount == 1)
            {
                firstMarkN = eggCount;
                return;
            }
            if (markCount == 2)
            {
                firstMarkN = GetUpRoundedHalfValue(eggCount);
                lastMarkN = eggCount / 2;
                return;
            }

            int remainEggs = eggCount - markCount;
            int gabs = markCount - 1;
            averageEggsInEachGab = (double)remainEggs / (double)gabs;
            firstMarkN = EggsCount(0);
            lastMarkN = eggCount - EggsCount(markCount - 2);
        }

        private void GetEggsIndexFromMarkIndex(int markIndex, out int eggIndexFrom, out int eggIndexTo)
        {
            if (markIndex == 0)
            {
                eggIndexFrom = 0;
                eggIndexTo = firstMarkN - 1;
                return;
            }
            if (markIndex == markCount - 1)
            {
                eggIndexTo = eggCount - 1;
                eggIndexFrom = eggIndexTo - lastMarkN + 1;
                return;
            }
            eggIndexFrom = EggsCount(markIndex - 1);
            eggIndexTo = EggsCount(markIndex) - 1;
        }

        private void GetMarkIndexFromEggIndex(int eggIndex, out int markIndex)
        {
            if (eggIndex < firstMarkN)
            {
                markIndex = 0;
                return;
            }
            if (eggIndex >= (eggCount - lastMarkN))
            {
                markIndex = markCount - 1;
                return;
            }

            //double rst = ((double)eggIndex + averageEggsInEachGab * 0.5) / (1 + averageEggsInEachGab);
            //markIndex = (int)rst;

            markIndex = (int)(((double)eggIndex + averageEggsInEachGab * 0.5) / (1 + averageEggsInEachGab));
            if (!IsRightEgg(markIndex, eggIndex))
            {
                if (markIndex == 1)
                    markIndex++;
                else if (markIndex == markCount - 2)
                    markIndex--;
                else
                {
                    if (IsRightEgg(markIndex - 1, eggIndex))
                        markIndex--;
                    else if (IsRightEgg(markIndex + 1, eggIndex))
                        markIndex++;
                    else
                    {
                        string msg = "Inner error in CutePointAndValuePresenter.GetMarkIndexFromEggIndex(), " +
                            "\r\n" + "can't find the correct markindex, please contact the author and " +
                            "report this error with the following data: " + "\r\n" +
                            "eggCount: " + eggCount.ToString() + "\r\n" +
                            "markCount: " + markCount.ToString() + "\r\n" +
                            "eggIndex: " + eggIndex.ToString();

                        throw new Exception(msg);
                    }

                }
            }
        }

        private bool IsRightEgg(int markIndex, int eggIndex)
        {
            int eggIndex1 = EggsCount(markIndex - 1);
            int eggIndex2 = EggsCount(markIndex) - 1;
            return (eggIndex1 <= eggIndex) && (eggIndex <= eggIndex2);
        }

        #endregion
    }
}
