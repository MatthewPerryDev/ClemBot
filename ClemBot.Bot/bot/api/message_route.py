import typing as t

import discord

from api.api_client import ApiClient
from api.base_route import BaseRoute


class MessageRoute(BaseRoute):

    def __init__(self, api_client: ApiClient):
        super().__init__(api_client)

    async def create_message(self, message_id: int, content: str, guild_id: int, author_id: int, channel_id: int):
        json = {
            'Id': message_id,
            'Content': content,
            'GuildId': guild_id,
            'UserId': author_id,
            'ChannelId': channel_id
        }
        await self.client.post('messages', data=json)

    async def edit_message(self, message_id: int, content: str):
        json = {
            'Id': message_id,
            'Content': content,
        }

        await self.client.patch('messages', data=json)

    async def get_message(self, message_id: int):
        return (await self.client.get(f'messages/{message_id}')).value
