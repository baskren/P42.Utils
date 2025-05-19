#if !WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static FormFactor GetDeviceForm()
    {
        if (_deviceForm != FormFactor.NotSet)
            return _deviceForm;

        var form = Windows.System.Profile.AnalyticsInfo.DeviceForm;
        if (Enum.TryParse<FormFactor>(form, true, out var factor))
            return _deviceForm = factor;

        return _deviceForm = FormFactor.Unknown;
    }

}
#endif
