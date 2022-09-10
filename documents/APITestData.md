# Test Data for API Testing

Guid Authentication Token:

```test
Key: Authorization Value: GuidTokenAuthentication OTk5OTk5OTktOTk5OS05OTk5LTk5OTktOTk5OTk5OTk5OTk5
```

Example API Query:

*api/dvoweatherforecast/listquery*

Filtering on a specific `WeatherLocationId` and sorting on `TemperatureC` descending.

```json
{
    "startIndex": 0,
    "pageSize": 10,
    "sortDescending": true,
    "sortExpressionString": "{\"__type\":\"LambdaExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":18,\"Type\":{\"GenericArguments\":[{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},{\"Name\":\"System.Object\"}],\"Name\":\"System.Func`2\"},\"Body\":{\"__type\":\"UnaryExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":10,\"Type\":{\"Name\":\"System.Object\"},\"Operand\":{\"__type\":\"MemberExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":23,\"Type\":{\"Name\":\"System.Int32\"},\"Expression\":{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"},\"Member\":{\"DeclaringType\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Signature\":\"Int32 TemperatureC\"}}},\"Parameters\":[{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"}]}",
    "filterExpressionString": "{\"__type\":\"LambdaExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":18,\"Type\":{\"GenericArguments\":[{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},{\"Name\":\"System.Boolean\"}],\"Name\":\"System.Func`2\"},\"Body\":{\"__type\":\"BinaryExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":13,\"Type\":{\"Name\":\"System.Boolean\"},\"Left\":{\"__type\":\"MemberExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":23,\"Type\":{\"Name\":\"System.Guid\"},\"Expression\":{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"},\"Member\":{\"DeclaringType\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Signature\":\"System.Guid WeatherLocationId\"}},\"Method\":{\"DeclaringType\":{\"Name\":\"System.Guid\"},\"Signature\":\"Boolean op_Equality(System.Guid, System.Guid)\"},\"Right\":{\"__type\":\"ConstantExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":9,\"Type\":{\"Name\":\"System.Guid\"},\"Value\":\"00000009-0000-0000-0000-000000000002\"}},\"Parameters\":[{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"}]}"
}
```

It will return 0 rows.  You need to change out `\"Value\":\"be984cd6-0f0b-4ff1-918e-c294dd8738b6\"}}` for a valid guid.

This returned:

```json
{
  "items": [
    {
      "uid": "cd486d2a-e413-460c-8ec4-61a4a4aec07f",
      "weatherSummaryId": "00295650-b9dd-42ac-93ab-05141aeecede",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-11-26T10:16:04.5677309+00:00",
      "temperatureC": 54,
      "summary": "Bracing",
      "location": "Capestang"
    },
    {
      "uid": "67b3e29f-faf4-4373-b537-5c68631ddc82",
      "weatherSummaryId": "8d803479-e93f-43f3-875b-bcbb22acdd18",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-10-16T10:16:04.5677011+01:00",
      "temperatureC": 52,
      "summary": "Chilly",
      "location": "Capestang"
    },
    {
      "uid": "49cf826b-eef6-428b-8669-bb79aacb381b",
      "weatherSummaryId": "1faddc92-bbbd-4628-9db6-76bc88b6688e",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-11-01T10:16:04.5677161+00:00",
      "temperatureC": 51,
      "summary": "Hot",
      "location": "Capestang"
    },
    {
      "uid": "4fbd6e23-fc5c-4939-9be6-db563f3498e5",
      "weatherSummaryId": "1faddc92-bbbd-4628-9db6-76bc88b6688e",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-11-08T10:16:04.5677203+00:00",
      "temperatureC": 51,
      "summary": "Hot",
      "location": "Capestang"
    },
    {
      "uid": "b4242c12-bcef-4061-ab54-6b10f96509c7",
      "weatherSummaryId": "37b96e8a-ec3b-44ff-a236-1a51e91da6fb",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-12-14T10:16:04.5677418+00:00",
      "temperatureC": 51,
      "summary": "Sweltering",
      "location": "Capestang"
    },
    {
      "uid": "56c57931-74f2-4e2c-ae65-d7287150f598",
      "weatherSummaryId": "f04bdf71-5adb-45eb-a3ab-91f64c8d7f5d",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-10-06T10:16:04.5676948+01:00",
      "temperatureC": 49,
      "summary": "Scorching",
      "location": "Capestang"
    },
    {
      "uid": "9993a991-9769-4e70-ac85-4cbae4c77cbd",
      "weatherSummaryId": "ca7f00ff-11df-4942-a70e-0eb970b78fd3",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-10-28T10:16:04.5677138+01:00",
      "temperatureC": 49,
      "summary": "Mild",
      "location": "Capestang"
    },
    {
      "uid": "d6a8ccea-dd14-464c-87c5-3e383924c522",
      "weatherSummaryId": "00295650-b9dd-42ac-93ab-05141aeecede",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-09-30T10:16:04.567691+01:00",
      "temperatureC": 48,
      "summary": "Bracing",
      "location": "Capestang"
    },
    {
      "uid": "9deaa8c8-501b-479f-920c-dd2f75bb95ce",
      "weatherSummaryId": "00295650-b9dd-42ac-93ab-05141aeecede",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-10-24T10:16:04.5677111+01:00",
      "temperatureC": 48,
      "summary": "Bracing",
      "location": "Capestang"
    },
    {
      "uid": "07736135-37e4-4e2f-b352-4d384e4d49b5",
      "weatherSummaryId": "8d803479-e93f-43f3-875b-bcbb22acdd18",
      "weatherLocationId": "00000009-0000-0000-0000-000000000002",
      "date": "2022-11-17T10:16:04.5677256+00:00",
      "temperatureC": 48,
      "summary": "Chilly",
      "location": "Capestang"
    }
  ],
  "totalItemCount": 100,
  "success": true,
  "message": "The query completed successfully"
}
```


