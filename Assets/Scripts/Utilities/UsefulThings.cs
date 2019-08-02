using UnityEngine;
using System.Collections;

namespace UsefulThings {

    public class ArrayFunctions {

        public static int[,] RotatedArray(int[,] a) {
            int szx = a.GetLength(0);
            int szy = a.GetLength(1);
            int[,] newboy = new int[szy, szx];

            string printout = "";

            for (int r = 0; r < szx; r++) {
                for (int c = 0; c < szy; c++) {
                    newboy[c, szx - 1 - r] = a[r, c];
                    printout += a[r, c];
                }
                printout += "\n";
            }

            return newboy;
        }

        public static int[,] RotatedArray(int[,] a, float degrees) {
            int times = ((int)degrees % 360) / 90;
            int[,] newboy = a;

            for (int t = 0; t < times; t++)
                newboy = RotatedArray(newboy);

            return newboy;
        }

        public static int[] CombineArrays(params int[][] arrays) {

            int size = int.MaxValue;

            foreach(int[] a in arrays)
                if (a.Length < size)
                    size = a.Length;

            int[] newArray = new int[size];

            for (int x = 0; x < size; x++) {

                newArray[x] = 0;
                foreach (int[] a in arrays)
                    newArray[x] = a[x];

            }

            return newArray;

        }

    }

}