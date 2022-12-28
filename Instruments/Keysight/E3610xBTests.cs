﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Agilent.CommandExpert.ScpiNet.AgE3610XB_1_0_0_1_00;
using ABTTestLibrary.AppConfig;
using ABTTestLibrary.Instruments;
using ABTTestLibrary.Instruments.Keysight;

namespace ABTTestLibraryTests.Instruments.Keysight {
    [TestClass()]
    public class E3610xBTests {
        public AgE3610XB AgE3610XB;
        public static Dictionary<String, Instrument> Instruments;
        public static DialogResult dr;

        [ClassInitialize]
        public static void ClassInitialize(TestContext TestContext) {
            dr = MessageBox.Show("Power ON all Instruments", "Power all ON.", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.Cancel) Assert.Inconclusive();
            Instruments = Instrument.Get();
        }

        [TestMethod()]
        public void RemoteOnTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                E3610xB.RemoteOn(i.Value);
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.SYSTem.COMMunicate.RLSTate.Query(out String RemoteLockState);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Remote Lock State: {RemoteLockState}"));
                Assert.AreEqual(RemoteLockState, "REM");
            }
        }

        [TestMethod()]
        public void RemoteLockOnTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                E3610xB.RemoteLockOn(i.Value);
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.SYSTem.COMMunicate.RLSTate.Query(out String RemoteLockState);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Remote Lock State: {RemoteLockState}"));
                Assert.AreEqual(RemoteLockState, "RWL");
                AgE3610XB.SCPI.SYSTem.REMote.Command();
            }
        }

        [TestMethod()]
        public void IsOffTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.OUTPut.STATe.Command(false);
                AgE3610XB.SCPI.OUTPut.STATe.Query(out Boolean IsOn);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Is Powered?: {IsOn}"));
                Assert.IsTrue(E3610xB.IsOff(i.Value));
            }
        }

        [TestMethod()]
        public void IsOnTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.SOURce.VOLTage.SENSe.SOURce.Command("INTernal");
                AgE3610XB.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command(0.5);
                AgE3610XB.SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command(0.5);
                AgE3610XB.SCPI.SOURce.CURRent.PROTection.STATe.Command(true);
                AgE3610XB.SCPI.OUTPut.STATe.Command(true);
                Thread.Sleep(100);
                AgE3610XB.SCPI.OUTPut.STATe.Query(out Boolean IsOn);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Is Powered?: {IsOn}"));
                Assert.IsTrue(E3610xB.IsOn(i.Value));
                AgE3610XB.SCPI.OUTPut.STATe.Command(false);
            }
        }

        [TestMethod()]
        public void OffTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                E3610xB.Off(i.Value);
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.OUTPut.STATe.Query(out Boolean IsOn);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Is Powered?: {IsOn}"));
                Assert.IsFalse(E3610xB.IsOn(i.Value));
            }
        }

        [TestMethod()]
        public void ONTest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                E3610xB.ON(i.Value, 0.5, 0.5);
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.OUTPut.STATe.Query(out Boolean IsOn);
                AgE3610XB.SCPI.MEASure.VOLTage.DC.Query(out Double V);
                AgE3610XB.SCPI.MEASure.CURRent.DC.Query(out Double A);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"Is Powered?: {IsOn}, V: {V}, A: {A}"));
                Assert.IsTrue((0.45 < V) && (V < 0.55));
                Assert.IsTrue((-0.05 < A) && (A < 0.55));
                Assert.IsTrue(IsOn);
                AgE3610XB.SCPI.OUTPut.STATe.Command(false);
            }
        }

        [TestMethod()]
        public void MeasureVATest() {
            foreach (KeyValuePair<String, Instrument> i in Instruments) {
                AgE3610XB = new AgE3610XB(i.Value.Address);
                AgE3610XB.SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command(0.5);
                AgE3610XB.SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command(0.5);
                AgE3610XB.SCPI.SOURce.CURRent.PROTection.STATe.Command(true);
                AgE3610XB.SCPI.OUTPut.STATe.Command(true);
                Thread.Sleep(100);
                (Double V, Double A) = E3610xB.MeasureVA(i.Value);
                Console.WriteLine(InstrumentTasks.GetMessage(i.Value, $"V: {V}, A: {A}"));
                Assert.IsTrue((0.45 < V) && (V < 0.55));
                Assert.IsTrue((-0.05 < A) && (A < 0.55));
                AgE3610XB.SCPI.OUTPut.STATe.Command(false);
            }
        }
    }
}