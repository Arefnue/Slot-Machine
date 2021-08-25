using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SlotMachine;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SlotMachineTest
    {
        // // A Test behaves as an ordinary method
        // [Test]
        // public void SlotMachineTestSimplePasses()
        // {
        //     
        //     Assert.True();
        // }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator SlotMachineTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            var gameObject = new GameObject();
            var slotMachine = gameObject.AddComponent<SlotMachineManager>();
            
            slotMachine.CalculateScoreChances();
            
            yield return null;
        }
    }
}
