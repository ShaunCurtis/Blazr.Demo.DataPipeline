# Test Data for API Testing

Example API Query:

*api/dvoweatherforecast/listquery*

Filtering on a specific `WeatherLocationId` and sorting on `TemperatureC` descending.

```json
{
    "startIndex": 0,
    "pageSize": 10,
    "sortDescending": true,
    "sortExpressionString": "{\"__type\":\"LambdaExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":18,\"Type\":{\"GenericArguments\":[{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},{\"Name\":\"System.Object\"}],\"Name\":\"System.Func`2\"},\"Body\":{\"__type\":\"UnaryExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":10,\"Type\":{\"Name\":\"System.Object\"},\"Operand\":{\"__type\":\"MemberExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":23,\"Type\":{\"Name\":\"System.Int32\"},\"Expression\":{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"},\"Member\":{\"DeclaringType\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Signature\":\"Int32 TemperatureC\"}}},\"Parameters\":[{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"}]}",
    "filterExpressionString": "{\"__type\":\"LambdaExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":18,\"Type\":{\"GenericArguments\":[{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},{\"Name\":\"System.Boolean\"}],\"Name\":\"System.Func`2\"},\"Body\":{\"__type\":\"BinaryExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":13,\"Type\":{\"Name\":\"System.Boolean\"},\"Left\":{\"__type\":\"MemberExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":23,\"Type\":{\"Name\":\"System.Guid\"},\"Expression\":{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"},\"Member\":{\"DeclaringType\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Signature\":\"System.Guid WeatherLocationId\"}},\"Method\":{\"DeclaringType\":{\"Name\":\"System.Guid\"},\"Signature\":\"Boolean op_Equality(System.Guid, System.Guid)\"},\"Right\":{\"__type\":\"ConstantExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":9,\"Type\":{\"Name\":\"System.Guid\"},\"Value\":\"be984cd6-0f0b-4ff1-918e-c294dd8738b6\"}},\"Parameters\":[{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"Type\":{\"Name\":\"Blazr.App.Core.DvoWeatherForecast\"},\"Name\":\"item\"}]}"
}
```

It will return 0 rows.  You need to change out `\"Value\":\"be984cd6-0f0b-4ff1-918e-c294dd8738b6\"}}` for a valid guid.

This returned:

```json
{
    "items": [
        {
            "uid": "fcf6951f-5af1-4212-98d9-032e23e3d0d0",
            "weatherSummaryId": "d5f24d52-63ee-4c9e-85d5-f6ff77420ec3",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-11-08T17:22:37.916695+00:00",
            "temperatureC": 54,
            "summary": "Chilly",
            "location": "Alvor"
        },
        {
            "uid": "13873eff-6fd0-4faf-bb40-b2393b0b0548",
            "weatherSummaryId": "a1ce36c6-22c4-4722-9339-a24c123f78fc",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-09-26T17:22:37.916668+01:00",
            "temperatureC": 52,
            "summary": "Sweltering",
            "location": "Alvor"
        },
        {
            "uid": "21fec0d5-5dd9-4e31-9c2a-91fcfe83292d",
            "weatherSummaryId": "67928e1f-1eea-498c-a60b-a552c6090dde",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-12-11T17:22:37.9167128+00:00",
            "temperatureC": 52,
            "summary": "Cool",
            "location": "Alvor"
        },
        {
            "uid": "f4e61cec-9555-429d-8c3e-2dd31afce202",
            "weatherSummaryId": "0901b7a4-7b1c-4bff-b3b5-2acaf0be2032",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-12-05T17:22:37.9167097+00:00",
            "temperatureC": 51,
            "summary": "Bracing",
            "location": "Alvor"
        },
        {
            "uid": "f506b441-1ccc-4528-a0c7-813c04d7e662",
            "weatherSummaryId": "d5f24d52-63ee-4c9e-85d5-f6ff77420ec3",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-12-07T17:22:37.9167108+00:00",
            "temperatureC": 51,
            "summary": "Chilly",
            "location": "Alvor"
        },
        {
            "uid": "28457029-f93e-430f-bd80-896039478047",
            "weatherSummaryId": "c6b23780-23e0-430f-bffb-c85bf9ac06c8",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-12-08T17:22:37.9167113+00:00",
            "temperatureC": 51,
            "summary": "Hot",
            "location": "Alvor"
        },
        {
            "uid": "d0187629-ab2e-4db2-9281-8315cf4439ed",
            "weatherSummaryId": "0e73ece3-0685-4ddd-9f0a-85078d3e0fcc",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-11-19T17:22:37.9167011+00:00",
            "temperatureC": 48,
            "summary": "Mild",
            "location": "Alvor"
        },
        {
            "uid": "777b1f59-2eae-4ea1-b0b4-bc7691fb9629",
            "weatherSummaryId": "7b4c8d93-d8b2-4531-9b49-e653043a5bce",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-09-19T17:22:37.9166641+01:00",
            "temperatureC": 47,
            "summary": "Balmy",
            "location": "Alvor"
        },
        {
            "uid": "659989fb-6cc6-490f-b336-c3c30f778ba6",
            "weatherSummaryId": "ce1cd67b-158e-4eb9-832e-2a07dbabcbce",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-10-01T17:22:37.9166707+01:00",
            "temperatureC": 47,
            "summary": "Warm",
            "location": "Alvor"
        },
        {
            "uid": "1c72c1d9-a355-41b4-a32a-4374f94733f7",
            "weatherSummaryId": "ce1cd67b-158e-4eb9-832e-2a07dbabcbce",
            "weatherLocationId": "be984cd6-0f0b-4ff1-918e-c294dd8738b6",
            "date": "2022-11-27T17:22:37.9167053+00:00",
            "temperatureC": 47,
            "summary": "Warm",
            "location": "Alvor"
        }
    ],
    "totalItemCount": 100,
    "success": true,
    "message": "The query completed successfully"
}```


