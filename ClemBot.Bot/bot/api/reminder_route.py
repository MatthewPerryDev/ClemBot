from bot.utils.converters import Duration
from datetime import datetime
import re

from bot.api.api_client import ApiClient
from bot.api.base_route import BaseRoute


class ReminderRoute(BaseRoute):

    def __init__(self, api_client: ApiClient):
        super().__init__(api_client)

    async def create_reminder(self, userId: int, link: str, wait: datetime, **kwargs):
        json = {
            'UserId': userId,
            'Link': link,
            'Time': wait,
        }
        return await self._client.post('reminders', data=json, **kwargs)

    async def delete_reminder(self, reminderId: int, **kwargs):
        json = {
            'Id': reminderId,
        }
        await self._client.delete('reminders', data=json,**kwargs)

    async def retrieve_reminder(self, reminderId: int):
        json = {
            'Id': reminderId,
        }
        return await self._client.get('reminders', data=json)

    async def bulk_retrieve_reminder(self):
        reminders = await self._client.get('reminders/BulkRetrieve')

        if not reminders:
            return

        return reminders