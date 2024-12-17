// telemetry.ts
import { ApplicationInsights, DistributedTracingModes } from '@microsoft/applicationinsights-web';

const instrumentationKey = process.env.REACT_APP_APPINSIGHTS_INSTRUMENTATION_KEY;

if (!instrumentationKey) {
  console.error('REACT_APP_APPINSIGHTS_INSTRUMENTATION_KEY is not defined.');
}

const appInsights = new ApplicationInsights({
  config: {
    instrumentationKey: instrumentationKey || 'b23dbc7c-7d33-4a9e-979f-2aa6e25c1492', // Fallback to empty string or handle appropriately
    enableAutoRouteTracking: true,
    distributedTracingMode: DistributedTracingModes.AI_AND_W3C,
    enableCorsCorrelation: true,
    enableDebug: true,
  },
});

appInsights.loadAppInsights();

export default appInsights;