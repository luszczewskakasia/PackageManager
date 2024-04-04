using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using RESTfulHTTPServer.src.models;
using RESTfulHTTPServer.src.controller;

namespace RESTfulHTTPServer.src.invoker
{
    public class PackagesInvoker
    {
        private const string TAG = "Packages Invoker";

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
                        MenagePackeges Packages_List = gameObject.GetComponent<MenagePackeges>();

                        responseData = JsonUtility.ToJson(Packages_List);
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




        public static Response Set(Request request)
        {
            Response response = new Response();
            string responseData = "";
            string json = request.GetPOSTData();
            string objname = request.GetParameter("objname");
            bool valid = true;

            UnityInvoker.ExecuteOnMainThread.Enqueue(() => {

                // Determine our object in the scene
                GameObject gameObject = GameObject.Find(objname);
                if (gameObject != null)
                {
                    try
                    {

                        // Deserialise the material
                        MenagePackeges Packages_List = JsonUtility.FromJson<MenagePackeges>(json);
                        //MenagePackeges Packages_List_res = new MenagePackeges();
                        // Check if it's our light source
                        //if (gameObject.GetComponent<MenagePackeges>() != null)
                        //{
                        //    // Set the color to the object
                        //    MenagePackeges managePack = gameObject.GetComponent<MenagePackeges>();
                        //    managePack.SetList(Packages_List.GetList());

                        //    Packages_List_res.SetList(Packages_List.GetList());
                        //    responseData = JsonUtility.ToJson(Packages_List_res);
                        responseData = json;
                        //}
                    }
                    catch (Exception e)
                    {
                        valid = false;
                        string msg = e.ToString();
                        responseData = msg;

                        RESTfulHTTPServer.src.controller.Logger.Log(TAG, msg);
                        RESTfulHTTPServer.src.controller.Logger.Log(TAG, e.ToString());
                    }

                }
                else
                {

                    // 404 - Object not found
                    responseData = "404";
                    response.SetContent(responseData);
                    response.SetHTTPStatusCode((int)HttpStatusCode.NotFound);
                    response.SetMimeType(Response.MIME_CONTENT_TYPE_HTML);
                }
            });

            // Wait for the main thread
            while (responseData.Equals("")) { }

            // Filling up the response with data
            if (valid)
            {

                // 200 - OK
                response.SetContent(responseData);
                response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                response.SetMimeType(Response.MIME_CONTENT_TYPE_JSON);
            }
            else
            {

                // 406 - Not acceptable
                response.SetContent("Failed to deseiralised JSON");
                response.SetHTTPStatusCode((int)HttpStatusCode.NotAcceptable);
                response.SetMimeType(Response.MIME_CONTENT_TYPE_HTML);
            }

            return response;
        }
    }
}




