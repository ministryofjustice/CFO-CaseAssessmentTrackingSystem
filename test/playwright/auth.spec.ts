import { test, expect } from '@playwright/test';

test('is alive', async ({ page }) => {
  await page.goto('/');

  // Expect a title "to contain" a substring.
  await expect(page).toHaveTitle("Case Assessment and Tracking System (CATS)");
});

/*
test('get started link', async ({ page }) => {
  await page.goto('/');

  // Click the get started link.
  await page.getByRole('link', { name: 'Get started' }).click();

  // Expects page to have a heading with the name of Installation.
  await expect(page.getByRole('heading', { name: 'Installation' })).toBeVisible();
});
*/