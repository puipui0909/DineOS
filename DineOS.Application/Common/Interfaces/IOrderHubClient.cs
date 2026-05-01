using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces;
public interface IOrderHubClient
{
    Task OrderCreated(object order);
    Task OrderUpdated(object order);
    Task NotifyNewItems(object data);
}