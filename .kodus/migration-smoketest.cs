// Throwaway file to verify the Kodus code-review pipeline after migrating
// the Kodus instance from server .25 to .176. Safe to delete.
using System;

namespace SplatDev.Kodus.SmokeTest
{
    public static class MigrationSmokeTest
    {
        // Intentionally trivial so any review comment confirms the pipeline ran.
        public static int Add(int a, int b)
        {
            var result = a + b;
            return result;
        }
    }
}
