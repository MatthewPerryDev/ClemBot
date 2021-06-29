import logging
from datetime import datetime

import discord
from marshmallow.fields import DateTime

from bot.consts import Colors
from bot.messaging.events import Events
from bot.services.base_service import BaseService
from bot.utils import converters

log = logging.getLogger(__name__)


class RemindService(BaseService):
    def __init__(self, *, bot):
        super().__init__(bot)
        
    @BaseService.Listener(Events.on_set_reminder)
    async def on_set_reminder(self, userId: int, wait: DateTime, link: str):
        reminderId: int = await self.bot.reminder_route.create_reminder(userId, link, wait.strftime('%Y-%m-%dT%H:%M:%S.%f'))

        self.bot.scheduler.schedule_at(self.reminder_callback(reminderId), time=wait)

    async def reminder_callback(self, reminderId: int):     

        data = await self.bot.reminder_route.retrieve_reminder(reminderId)
        
        user: discord.User = self.bot.get_user(data['userId'])

        time = data['time']
    
        embed = discord.Embed(title='⏰Reminder', color = Colors.ClemsonOrange)
        embed.add_field(name='Message Date', value = time)
        embed.add_field(name='Message Link', value = "[message link](" + data['link'] + ")", inline=False)
        await user.send(embed = embed)
        
        await self.bot.reminder_route.delete_reminder(reminderId)

    async def load_service(self):
        reminders = await self.bot.reminder_route.bulk_retrieve_reminder()
        if not reminders:
            return 
        for reminder in reminders:
            wait: datetime = datetime.strptime(reminder['time'], '%Y-%m-%dT%H:%M:%S.%f')
            if (wait - datetime.utcnow()).total_seconds() <= 0:
                await self.reminder_callback(reminderId=reminder['id'])
            else:
                self.bot.scheduler.schedule_at(self.reminder_callback(reminder['id']), time=wait)