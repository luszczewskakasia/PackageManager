using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using RESTfulHTTPServer.src.models;
using RESTfulHTTPServer.src.controller;
using Newtonsoft.Json;

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

                        responseData = simComponent.ToJson();
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
                        Simulation sim = JsonConvert.DeserializeObject<Simulation>(json);

                        Simulation Current_sim = gameObject.GetComponent<Simulation>();
                        Current_sim.SetList(sim.GetList());
                        Current_sim.sort_method = sim.sort_method;
                        Current_sim.set_start(sim.Line_start_x, sim.Line_start_y);

                        //    Packages_List_res.SetList(Packages_List.GetList());
                        //    responseData = JsonUtility.ToJson(Packages_List_res);
                        responseData = json;
                        response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                        //}
                    }
                    catch (Exception e)
                    {
                        valid = false;
                        string msg = "Error: " + e.ToString();
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


        public static Response Put(Request request)
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
                        warehouse Whouse = JsonConvert.DeserializeObject<warehouse>(json);

                        Simulation Current_sim = gameObject.GetComponent<Simulation>();
                        Current_sim.Add_warehouse(Whouse);


                        responseData = json;
                        response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                    }
                    catch (Exception e)
                    {
                        valid = false;
                        string msg = "Error: " + e.ToString();
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


        public static Response Delete(Request request) 
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
                        ID_List_To_Delete ids = JsonConvert.DeserializeObject<ID_List_To_Delete>(json);
                        Simulation Current_sim = gameObject.GetComponent<Simulation>();

                        foreach (int i in ids.IDs)
                        {
                            Current_sim.Delete_Warehouse(i);
                        }
                        responseData = json;
                        response.SetHTTPStatusCode((int)HttpStatusCode.OK);
                    }
                    catch (Exception e)
                    {
                        valid = false;
                        string msg = "Error: " + e.ToString();
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
