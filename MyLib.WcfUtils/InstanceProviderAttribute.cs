using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WcfUtils
{


  public class InstanceProviderBehaviorAttribute : Attribute, IServiceBehavior
  {
    public InstanceProviderBehaviorAttribute(Type providerType)
    {
      ProviderType = providerType;
    }

    public Type ProviderType { get; private set;}

    public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
      foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
      {
        foreach (EndpointDispatcher ed in cd.Endpoints)
        {
          if (!ed.IsSystemEndpoint)
          {
            ConstructorInfo constructor = ProviderType.GetConstructor(new Type[0]);
            ed.DispatchRuntime.InstanceProvider = (IInstanceProvider)constructor.Invoke(new object[0]);
              //new ServiceInstanceProvider();
          }
        }
      }
    }

    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
    }
  }

}
