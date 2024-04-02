using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using RESTfulHTTPServer.src.models;
using RESTfulHTTPServer.src.controller;

namespace RESTfulHTTPServer.src.invoker
{
    public class SimulationInvoker
    {
        private const string TAG = "Simulation Invoker";

        /// <summary>
        /// Get the color of an object
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="request">Request.</param>
        public static Response Get(Request request)
        {
            Response response = new Response();
            string objname = request.GetParameter("objname");
            string responseData = "";

            // Verbose all URL variables
            foreach (string key in request.GetQuerys().Keys)
            {

                string value = request.GetQuery(key);
                RESTfulHTTPServer.src.controller.Logger.Log(TAG, "key: " + key + " , value: " + value);
            }

            UnityInvoker.ExecuteOnMainThread.Enqueue(() => {

                // Determine our object in the scene
                GameObject gameObject = GameObject.Find(objname);
                if (gameObject != null)
                {
                    try
                    {
                        Simulation simComponent = gameObject.GetComponent<Simulation>();

                        responseData = JsonUtility.ToJson(simComponent);
                        response.SetHTTPStatusCode((int)HttpStatusCode.OK);

                        
                    }
                    catch (Exception e)
                    {
                        string msg = "Failed to seiralised JSON";
                        responseData = msg;

                        RESTfulHTTPServer.src.controller.Logger.Log(TAG, msg);
                        RESTfulHTTPServer.src.controller.Logger.Log(TAG, e.ToString());
                    }

                }
                else
                {
                    // 404 - Not found
                    responseData = "404";
                    response.SetContent(responseData);
                    response.SetHTTPStatusCode((int)HttpStatusCode.NotFound);
                    response.SetMimeType(Response.MIME_CONTENT_TYPE_TEXT);
                }
            });

            // Wait for the main thread
            while (responseData.Equals("")) { }

            // 200 - OK
            // Fillig up the response with data
            response.SetContent(responseData);
            response.SetMimeType(Response.MIME_CONTENT_TYPE_JSON);

            return response;
        }




    }
}
