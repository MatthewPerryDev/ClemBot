from datetime import datetime

from bot.api.api_client import ApiClient
from bot.api.base_route import BaseRoute


class ReminderRoute(BaseRoute):

    def __init__(self, api_client: ApiClient):
        super().__init__(api_client)

    async def create_reminder(self, userId: int, messageId: int, link: str, wait: datetime, **kwargs):
        json = {
            'UserId': userId,
            'MessageId': messageId,
            'Link': link,
            'Wait': wait,
        }
        await self._client.post('reminders', data=json, **kwargs)

    async def delete_reminder(self, userId: int, messageId: int):
        json = {
            'UserId': userId,
            'MessageId': messageId,
        }
        await self._client.delete('reminders', data=json)

    async def retrieve_reminder(self, userId: int, messageId: int):
        json = {
            'UserId': userId,
            'MessageId': messageId,
        }
        return await self._client.get('reminders', data=json)

    async def bulk_retrieve_reminder(self):
        reminders = await self._client.get('reminders/BulkRetrieve')

        if not reminders:
            return

        return reminders