// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebInterface.cs" company="Supyrb">
//   Copyright (c) 2017 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------
 
using UnityEngine;
using System.Collections;
 
namespace Supyrb
{
    public class WebInterface : MonoBehaviour
    {
 
        public void OpenWebsite(string url)
        {
            Application.OpenURL(url);
        }
    }
}