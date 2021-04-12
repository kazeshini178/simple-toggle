using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using SimpleToggle.Core;

namespace SimpleToggle.Sources.AWS
{
    public class DynamoDBToggleSource : IToggleSource
    {
        private const string DYNAMODB_TOGGLE_NAME_COLUMN = "ToggleName";
        private const string DYNAMODB_TOGGLE_VALUE_COLUMN = "ToggleValue";
        private const string DYNAMODB_TABLE_NAME = "FeatureToggles";

        private readonly IAmazonDynamoDB dynamoDB;
        private readonly FeatureToggles toggles;

        public DynamoDBToggleSource(IAmazonDynamoDB dynamoDB, IOptions<FeatureToggles> toggles)
        {
            this.dynamoDB = dynamoDB;
            this.toggles = toggles.Value;
        }

        public async Task<bool> GetToggleValue(string toggleName)
        {
            if (!toggles.TryGetValue(toggleName, out var parameterName))
            {
                return false;
            }

            GetItemResponse response = await dynamoDB.GetItemAsync(new GetItemRequest()
            {
                Key = new Dictionary<string, AttributeValue>()
                {
                    [DYNAMODB_TOGGLE_NAME_COLUMN] = new AttributeValue(parameterName)
                },
                TableName = DYNAMODB_TABLE_NAME
            });

            // TODO: Consider if exception should be handled. As they are more around service limit user should be aware of.
            return response.Item.ContainsKey(DYNAMODB_TOGGLE_VALUE_COLUMN) ? response.Item[DYNAMODB_TOGGLE_VALUE_COLUMN].BOOL : false;
            //return response.Item.ContainsKey(DYNAMODB_TOGGLE_VALUE_COLUMN) && response.Item[DYNAMODB_TOGGLE_VALUE_COLUMN].BOOL;

        }
    }
}
