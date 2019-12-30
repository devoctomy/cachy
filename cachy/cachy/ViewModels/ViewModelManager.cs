using cachy.ViewModels.Attributes;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class ViewModelManager
    {

        #region private objects

        private static ViewModelManager _instance;
        private Dictionary<VisualElement, List<Object>> _viewModelCache;

        #endregion

        #region public properties

        public static ViewModelManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ViewModelManager();
                }
                return (_instance);
            }
        }

        #endregion

        #region constructor / destructor

        public ViewModelManager()
        {
            _viewModelCache = new Dictionary<VisualElement, List<Object>>();
        }

        #endregion

        #region public methods

        public Object GetViewModel(View view)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Getting view model for view type '{0}'.", view.GetType().Name);

            Object[] mappingAttributes = view.GetType().GetCustomAttributes(typeof(ViewModelMappingAttribute), true);
            if (mappingAttributes != null && mappingAttributes.Length == 1)
            {
                ViewModelMappingAttribute mappingAttribute = (ViewModelMappingAttribute)mappingAttributes[0];
                List<Object> modelList;
                if (!_viewModelCache.ContainsKey(view))
                {
                    modelList = new List<Object>();
                    _viewModelCache.Add(view, modelList);
                }
                else
                {
                    modelList = _viewModelCache[view];
                }
                switch (mappingAttribute.Instancing)
                {
                    case ViewModelMappingAttribute.InstancingType.Single:
                        {
                            if (modelList.Count == 1)
                            {
                                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Returning existing single instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                                return (modelList[0]);
                            }
                            else
                            {
                                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Creating first single instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                                Object viewModel = Activator.CreateInstance(mappingAttribute.ViewModelType, view);
                                modelList.Add(viewModel);
                                return (viewModel);
                            }
                        }
                    case ViewModelMappingAttribute.InstancingType.Multiple:
                        {
                            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Creating new multi-instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                            Object viewModel = Activator.CreateInstance(mappingAttribute.ViewModelType, view);
                            modelList.Add(viewModel);
                            return (viewModel);
                        }
                    default:
                        {
                            throw new InvalidOperationException(String.Format("Instancing type of '{0}' is invalid.", view.GetType().Name));
                        }
                }
            }
            else
            {
                return (null);
            }
        }

        public Object GetViewModel(Page page)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Getting view model for page type '{0}'.", page.GetType().Name);

            Object[] mappingAttributes = page.GetType().GetCustomAttributes(typeof(ViewModelMappingAttribute), true);
            if(mappingAttributes != null && mappingAttributes.Length == 1)
            {
                ViewModelMappingAttribute mappingAttribute = (ViewModelMappingAttribute)mappingAttributes[0];
                List<Object> modelList;
                if (!_viewModelCache.ContainsKey(page))
                {
                    modelList = new List<Object>();
                    _viewModelCache.Add(page, modelList);
                }
                else
                {
                    modelList = _viewModelCache[page];
                }
                switch(mappingAttribute.Instancing)
                {
                    case ViewModelMappingAttribute.InstancingType.Single:
                        {
                            if(modelList.Count == 1)
                            {
                                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Returning existing single instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                                return (modelList[0]);
                            }
                            else
                            {
                                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Creating first single instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                                Object viewModel = Activator.CreateInstance(mappingAttribute.ViewModelType, page);
                                modelList.Add(viewModel);
                                return (viewModel);
                            }
                        }
                    case ViewModelMappingAttribute.InstancingType.Multiple:
                        {
                            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Creating new multi-instance of view model type '{0}'.", mappingAttribute.ViewModelType);

                            Object viewModel = Activator.CreateInstance(mappingAttribute.ViewModelType, page);
                            modelList.Add(viewModel);
                            return (viewModel);
                        }
                    default:
                        {
                            throw new InvalidOperationException(String.Format("Instancing type of '{0}' is invalid.", page.GetType().Name));
                        }
                }
            }
            else
            {
                return (null);
            }
        }

        #endregion

    }

}
